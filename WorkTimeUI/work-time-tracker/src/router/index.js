import { createRouter, createWebHistory } from 'vue-router';
import { user } from '@/auth.js';
import ProjectsView from '@/views/ProjectsView.vue';
import TasksList from '@/views/TasksList.vue';
import TransactionsList from '@/views/TransactionsList.vue';
import MainPage from '@/views/MainPage.vue';
import UserLogin from '@/views/UserLogin.vue';

const routes = [
  { path: '/projects', component: ProjectsView },
  { path: '/tasks/:projectID', component: TasksList },
  { path: '/tasks/', component: TasksList },
  { path: '/transactions/:taskID', component: TransactionsList },
  { path: '/transactions/', component: TransactionsList },
  { path: '/', component: MainPage },
  { path: '/login', component: UserLogin, meta: { layout: 'auth' } }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to) => {
  if (!user.value && to.path !== '/login') {
    return '/login'; // Если не авторизован, отправляем на вход
  }
  if (user.value && to.path === '/login') {
    return '/'; // Если уже авторизован, но хочет вдруг зайти на вход, отправляем на главную
  }
});

export default router;
