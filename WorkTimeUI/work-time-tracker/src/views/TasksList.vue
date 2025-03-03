<script setup>
import { useRouter, useRoute } from 'vue-router';
import { getTasks, deleteTask } from '@/services/tasks';
import { getProject } from '@/services/projects';
import TaskForm from '@/components/TaskForm.vue';
import { onMounted, ref, h, inject, watch } from 'vue';
import { NDataTable, NButton, NTag, useMessage, NModal, NLayout, NFlex } from 'naive-ui';

const router = useRouter();
const route = useRoute();
// Получаем ID проекта
const projectId = ref(route.params.projectID);
const tasks = ref([]);
// Задача, для которой откроется форма
const selectedTask = ref(null);
const message = useMessage();

// Ответ от бэка
const response = ref([]);
// Проекты, которым соответствует задача
const projects = ref({});
// Состояние модального окна
const showModal = inject('showModal');

const fetchTasks = async () => {
    try {
        const projectID = route.params.projectID || null;
        response.value = await getTasks(projectID);
        tasks.value = response.value.data.map((p, index) => ({
            index: index + 1,
            ...p
        }));
        await loadProjects();
        selectedTask.value = null;
    } catch (error) {
        message.error('Ошибка загрузки задач');
    }
};

onMounted(fetchTasks);

// Отслеживание изменений projectID
watch(
    () => route.params.projectID,
    (newProjectID) => {
        projectId.value = newProjectID;
        fetchTasks();
    }
);

// Удаление проекта
const removeTask = async (id) => {
    if (!confirm('Вы уверены, что хотите удалить задачу?')) return;
    try {
        await deleteTask(id);
        tasks.value = tasks.value.filter(p => p.id !== id);
        message.success('Задача удалена');
    } catch (error) {
        message.error('Ошибка удаления задачи');
    }
};

// Выбор проекта для редактирования
const selectTask = (task) => {
    if (selectedTask.value === null) {
        selectedTask.value = { ...task };
    }
    else {
        selectedTask.value = null;
    }
};

// Загрузка названий проектов
const loadProjects = async () => {
    try {
        const projectIDs = [...new Set(tasks.value.map(t => t.projectId))]; // Уникальные ID проектов
        const projectPromises = projectIDs.map(id => getProject(id));
        const projectResults = await Promise.all(projectPromises);
        projects.value = projectResults.reduce((acc, project, idx) => {
            acc[projectIDs[idx]] = project;
            return acc;
        }, {});
    } catch (error) {
        message.error('Ошибка загрузки проектов');
    }
};

const projectName = (id) => projects.value[id]?.data.name || 'Загрузка...';


// Колонки для таблицы
const columns = [
    {
        type: 'expand',
        expandable: (row) => {
            return selectedTask.value && row.id === selectedTask.value.id;
        },
        renderExpand: (row) => {
            return h(TaskForm, {
                task: row,
                onUpdate: fetchTasks,
                onClose: () => (selectedTask.value = null)
            });
        }
    },
    { title: '#', key: 'index', width: 'auto' },
    {
        title: 'Проект',
        width: 'auto',
        key: 'project',
        render: (row) => projectName(row.projectId)
        
    },
    { title: 'Название', key: 'name', width: 'auto' },
    {
        title: 'Активна',
        key: 'isActive',
        width: 'auto',
        align: 'center',
        render: (row) =>
            row.isActive
                ? h(NTag, { type: 'success' }, { default: () => '✔' })
                : h(NTag, { type: 'error' }, { default: () => '✘' })
    },
    {
        title: 'Действия',
        key: 'actions',
        width: 'auto',
        render: (row) =>
            h('div', { style: 'display: flex; gap: 20px; justify-content: flex-start;' }, [
                h(NButton, {
                    secondary: true,
                    type: 'primary',
                    size: 'small',
                    onClick: () => router.push(`/transactions/${row.id}`)
                }, () => 'К проводкам'),
                h(NButton, {
                    secondary: true,
                    type: 'warning',
                    size: 'small',
                    onClick: () => selectTask(row)
                }, () => 'Редактировать'),
                h(NButton, {    
                    secondary: true,                
                    type: 'error',
                    size: 'small',
                    onClick: () => removeTask(row.id)
                }, () => 'Удалить')
            ])
    }
];

</script>

<template>
    <n-layout style="padding-right: 10px; padding-left: 10px;">
            <n-flex justify="space-between" align="center">
                <h2>{{ projectId ? `Задачи проекта ${projectName(projectId)}` : 'Список задач' }}</h2>
                <n-button type="primary" size="medium" @click="() => showModal = !showModal">Добавить задачу</n-button>
            </n-flex>

            <n-data-table :columns="columns" :data="tasks" :bordered="false" :striped="true" default-expand-all style="margin-top: 10px;">
            </n-data-table>

            <!-- Модальное окно для добавления задачи -->
            <n-modal v-model:show="showModal" style="width: 1000px" preset="card" title="Добавление задачи"
                size="medium">
                <TaskForm :project="selectedTask" @update="fetchTasks" @close="() => showModal = !showModal" />
            </n-modal>
    </n-layout>
</template>
