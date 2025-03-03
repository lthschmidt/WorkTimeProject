<script setup>
import { NCard,  NCollapse, NCollapseItem, NDivider, NIcon, NTable, useMessage } from 'naive-ui';
import { SentimentDissatisfiedOutlined, SentimentSatisfiedOutlined, SentimentNeutralOutlined } from "@vicons/material";
import { user } from '@/auth.js';
import TransactionsList from '@/views/TransactionsList.vue';
import { getTransactions } from '@/services/transactions';
import { ref, onMounted, computed } from 'vue';

const message = useMessage();
const today = new Date();
// Дата для получения проводок пользователя ГГГГ-ММ-ДД 
const todayDashed = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`; 
// Дата для вывода пользователю ДД.ММ.ГГГГ
const todayDotted = today.toLocaleDateString('ru-RU');
// Проводки пользователя
const showTransactions = ref(false);
const transactions = ref([]);

// Загрузка проводок пользователя за сегодня
const loadTransactions = async () => {
  try {
    const response = await getTransactions(null, user.value?.id, todayDashed);
    transactions.value = response.data;
  } catch (error) {
    message.error('Ошибка загрузки проводок');
  }
};

onMounted(loadTransactions);

// Подсчет суммы часов за сегодня
const totalHours = computed(() => transactions.value.reduce((sum, t) => sum + (t.hours || 0), 0));


// Определение цвета и иконки в зависимости от количества часов
const hoursStatus = computed(() => {
  if (totalHours.value < 8) return { color: 'orange', icon: SentimentNeutralOutlined };
  if (totalHours.value === 8) return { color: 'green', icon: SentimentSatisfiedOutlined };
  return { color: 'red', icon: SentimentDissatisfiedOutlined };
});

</script>

<template>
  <div style="padding-right: 10px; padding-left: 10px;">
    <n-card title="Профиль" hoverable embedded>
    <n-table :bordered="false" style="--n-td-color: #ffffff00">
      <thead>
        <tr>
          <th>Имя</th>
          <th>Email</th>
          <th>Пароль</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ user?.name || 'Неизвестный' }}</td>
          <td>{{ user?.email || 'Нет данных' }}</td>
          <td>********</td>
        </tr>
      </tbody>
    </n-table>
  </n-card>

  <n-divider />

  <n-card title="Отчет за сегодня" hoverable embedded>
    <n-table :bordered="false" style="--n-td-color: #ffffff00">
      <thead>
        <tr>
          <th>Дата</th>
          <th>Отработано</th>
          <th>Статус</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ todayDotted }}</td>
          <td>{{ totalHours }}</td>
          <td>
            <n-icon :color="hoursStatus.color" size="30" :component="hoursStatus.icon" />
          </td>
        </tr>
      </tbody>
    </n-table>
  </n-card>
    <n-divider />

    <n-collapse @item-header-click="() => showTransactions = !showTransactions">
      <n-collapse-item title="Мои проводки" name="1">
        <KeepAlive>
          <TransactionsList v-if="showTransactions" @update="loadTransactions" />
        </KeepAlive>
      </n-collapse-item>
    </n-collapse>
  </div>
</template>

<style scoped>
.n-table {
  width: 100%;
}

th,
td {
  text-align: center;
  padding: 8px;
  width: 100px;
}

th {
  font-weight: bold;
}

td {
  background-color: var(--n-card-color); /* Чтоб строки не выбивались */
}
</style>