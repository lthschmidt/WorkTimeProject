<script setup>
import { useRoute } from 'vue-router';
import { getTransactions, deleteTransaction } from '@/services/transactions';
import { getUser } from '@/services/users';
import { getTask } from '@/services/tasks';
import TransactionForm from '@/components/TransactionForm.vue';
import { onMounted, ref, h, inject, watch, defineEmits } from 'vue';
import { NDataTable, NButton, useMessage, NModal, NLayout, NSpace, NFlex, NSelect, NDatePicker } from 'naive-ui';
import { user } from '@/auth.js';

const route = useRoute();
// Получаем ID проводки
const taskId = ref(route.params.taskID);
// Проводки от бэка
const transactions = ref([]);
const selectedTransaction = ref(null);
const message = useMessage();
const users = ref({});
const tasks = ref({});
 // Состояние модального окна
const showModal = inject('showModal');
const response = ref(null);
const emit = defineEmits(['update']);
// Проводки, которые выводятся пользователю
const filteredTransactions = ref([]);

// Фильтры
const filterType = ref('all');
const selectedDate = ref(null);
const filterOptions = [
    { label: 'Все', value: 'all' },
    { label: 'По году', value: 'year' },
    { label: 'По месяцу', value: 'month' },
    { label: 'По дню', value: 'date' }
];

// Загрузка проводок
const fetchTransactions = async () => {
    try {
        if (!taskId.value) {
            response.value = route.path === '/'
                ? await getTransactions(null, user.value.id, null)
                : await getTransactions(null, null, null);
        } else {
            response.value = await getTransactions(taskId.value, null, null);
        }

        transactions.value = response.value.data.map((p, index) => ({
            index: index + 1,
            ...p
        }));

        filteredTransactions.value = transactions.value;
        // Уникальные id пользователей и задач
        const userIds = [...new Set(transactions.value.map(t => t.userId))];
        const taskIds = [...new Set(transactions.value.map(t => t.taskId))];

        // Загрузка имен пользователей и названий задач
        await Promise.all([
            loadUsers(userIds),
            loadTasks(taskIds)
        ]);

    } catch (error) {
        message.error(error.response.data);
    }
};

onMounted(fetchTransactions);

// Следим за изменением taskID
watch(
    () => route.params.taskID,
    (newTaskID) => {
        taskId.value = newTaskID;
        fetchTransactions();
    }
);

// Удаление проводки
const removeTransaction = async (id) => {
    if (!confirm('Вы уверены, что хотите удалить проводку?')) return;
    try {
        await deleteTransaction(id);
        transactions.value = transactions.value.filter(p => p.id !== id);
        message.success('Проводка удалёна');
        emit('update');
        await fetchTransactions();
    } catch (error) {
        message.error(`Ошибка удаления проводки: ${error.response.data}`);
    }
};

// Загрузка имён пользователей, открывших проводки
const loadUsers = async (userIds) => {
    try {
        const requests = userIds.map(id => getUser(id));
        const results = await Promise.all(requests);
        results.forEach((user, index) => {
            users.value[userIds[index]] = user;
        });
    }
    catch (error)
    {
        message.error(error.response.data);
    }
};

// Загрузка названий задач
const loadTasks = async (taskIds) => {
    try {
        const requests = taskIds.map(id => getTask(id));
        const results = await Promise.all(requests);
        results.forEach((task, index) => {
            tasks.value[taskIds[index]] = task;
        });
    }
    catch (error)
    {
        message.error(error.response.data);
    }
};

// Функция фильтрации
const filterTransactions = () => {
    if (!selectedDate.value || filterType.value === 'all') {
        filteredTransactions.value = [...transactions.value];
        return;
    }

    // Разбиваем дату на год, месяц и день
    const selected = new Date(selectedDate.value);
    const selectedYear = selected.getFullYear();
    const selectedMonth = selected.getMonth();
    const selectedDay = selected.getDate();

    filteredTransactions.value = transactions.value.filter(t => {
        const transactionDate = new Date(t.date);
        const transactionYear = transactionDate.getFullYear();
        const transactionMonth = transactionDate.getMonth();
        const transactionDay = transactionDate.getDate();

        // Вощвращаем проводки, подходящие по выбранному фильтру
        if (filterType.value === 'year') {
            return transactionDate.getFullYear() === selected.getFullYear();
        }
        if (filterType.value === 'month') {
            return transactionDate.getFullYear() === selected.getFullYear() &&
                transactionDate.getMonth() === selected.getMonth();
        }
        if (filterType.value === 'date') {
            return (
                transactionYear === selectedYear &&
                transactionMonth === selectedMonth &&
                transactionDay === selectedDay
            );
        }
        return true;
    });
};

const taskName = (id) => tasks.value[id]?.data.name || 'Загрузка...';

// Колонки для таблицы
const columns = [
    {
        type: 'expand',
        expandable: (row) => {
            return selectedTransaction.value && row.id === selectedTransaction.value.id;
        },
        renderExpand: (row) => {
            return h(TransactionForm, {
                transaction: row,
                onUpdate: fetchTransactions,
                onClose: () => (selectedTransaction.value = null)
            });
        }
    },
    { title: '#', key: 'index', width: 'auto' },
    {
        title: 'Пользователь',
        key: 'user',
        width: 'auto',
        render: (row) => users.value[row.userId]?.data.name || 'Загрузка...'
    },
    {
        title: 'Задача',
        key: 'task',
        width: 'auto',
        render: (row) => taskName(row.taskId)
    },
    {
        title: 'Описание',
        key: 'description',
        ellipsis: false, // Отключаем обрезание текста
        width: 250,
        render(row) {
            return h(
                'div',
                { style: 'white-space: pre-wrap; word-break: break-word;' },
                row.description
            );
        }
    },
    { title: 'Часы', key: 'hours', width: 'auto' },
    { title: 'Дата', key: 'date', width: 'auto' },
    {
        title: 'Действия',
        key: 'actions',
        width: 'auto',
        render: (row) =>
            h('div', { style: 'display: flex; gap: 20px; justify-content: flex-start;' }, [
                h(NButton, {
                    secondary: true,
                    type: 'error',
                    size: 'small',
                    onClick: () => removeTransaction(row.id)
                }, () => 'Удалить')
            ])
    }
];

</script>

<template>
    <n-layout>
        <div class="container" style="padding-right: 10px; padding-left: 10px;">
            <n-flex justify="space-between" align="center">
                <h2 v-if="route.path !== '/'">
                    {{ taskId ? `Проводки задачи ${taskName(taskId)}` : 'Список проводок' }}
                </h2>
                <n-space >
                    <n-select v-model:value="filterType" :options="filterOptions" placeholder="Выберите фильтр"
                        style="width: 180px" />
                    <n-date-picker v-if="filterType !== 'all'" v-model:value="selectedDate" :type="filterType" clearable
                        @update:value="filterTransactions" style="width: 200px" />
                    <n-button type="primary" size="medium" @click="() => showModal = !showModal">Добавить проводку</n-button>
                </n-space>
            </n-flex>

            <n-data-table :columns="columns" :data="filteredTransactions" :bordered="false" :striped="true"
                default-expand-all style="margin-top: 10px;">
            </n-data-table>

            <!-- Модальное окно для добавления проводки -->
            <n-modal v-model:show="showModal" style="width: 600px" preset="card" title="Добавление проводки"
                size="medium">
                <TransactionForm :transaction="selectedTransaction" @update="fetchTransactions"
                    @close="() => { showModal = !showModal; emit('update') }" />
            </n-modal>
        </div>
    </n-layout>
</template>
