<script setup>
import { ref, provide, watch, onMounted, onUnmounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { NConfigProvider, NMessageProvider, NLayout, NMenu, NButton, NButtonGroup, NH1, darkTheme, NLayoutSider } from 'naive-ui';
import { logout } from '@/auth';

const activeMenu = ref('');
const router = useRouter();
const route = useRoute();
// Глобальная переменная для модального окна
const showModal = ref(false);
provide('showModal', showModal);

// Обновляем activeMenu при смене маршрута
const updateActiveMenu = () => {
  const path = route.path;
  if (path.startsWith('/projects')) activeMenu.value = 'projects';
  else if (path.startsWith('/tasks')) activeMenu.value = 'tasks';
  else if (path.startsWith('/transactions')) activeMenu.value = 'transactions';
  else {
    activeMenu.value = '';
  }
};

const handleLogout = () => {
  logout();
  router.push('/login');
};


const menuOptions = [
  { label: 'Профиль', key: '' },
  { label: 'Проекты', key: 'projects' },
  { label: 'Задачи', key: 'tasks' },
  { label: 'Проводки', key: 'transactions' }
];

// Реактивное состояние кнопки "Назад"
const canGoBack = ref(false);

// Проверка истории при изменении маршрута
const updateCanGoBack = () => {
  canGoBack.value = (router.options.history.state.back !== null);
};

// Отслеживаем изменения маршрута
watch(route, () => {
  updateActiveMenu();
  updateCanGoBack();
}, { immediate: true });

// Добавляем обработчик popstate (навигация назад/вперед)
onMounted(() => {
  window.addEventListener('popstate', updateCanGoBack);
  document.body.classList.add('dark-theme');
});
onUnmounted(() => window.removeEventListener('popstate', updateCanGoBack));

// Функция возврата назад
const goBack = () => {
  if (canGoBack.value) router.back();
};
</script>

<template>
  <n-config-provider :theme="darkTheme" >
    <n-message-provider>
      <div v-if="route.meta.layout === 'auth'">
        <!-- Только форма авторизации -->
        <router-view></router-view>
      </div>
      <div v-else :class="{ 'n-modal-blur': showModal }" >
        <n-layout has-sider>
          <n-layout-sider bordered content-style="padding: 20px;">
            <n-button-group>
              <n-button @click="handleLogout" type="error" round secondary size="small"
                style="margin-bottom: 15px;">Выйти</n-button>
              <n-button @click="goBack" type="primary" secondary size="small" :disabled="!canGoBack"
                style="margin-bottom: 15px;">Назад</n-button>
            </n-button-group>
            <n-menu v-model:value="activeMenu" :options="menuOptions" @update:value="router.push(`/${$event}`)" />
          </n-layout-sider>

          <n-layout content-style="padding: 20px;">
            <n-h1>Корпоративный портал учета рабочего времени</n-h1>
            <router-view></router-view>
          </n-layout>
        </n-layout>
      </div>
    </n-message-provider>
  </n-config-provider>
</template>

<style>
/* Эффект блюра при открытии модального окна */
.n-modal-blur {
  filter: blur(10px);
}

/* Стили для светлой темы */
body {
  background-color: #ffffff;
  color: #000000;
  transition: background-color 0.3s ease, color 0.3s ease;
}

/* Стили для темной темы */
body.dark-theme {
  background-color: #101014;
  color: #ffffff;
}

header {
  background-color: inherit;
  transition: background-color 0.3s ease;
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
