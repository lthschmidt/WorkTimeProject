<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { loginUser } from '@/services/users';
import { NInput, NButton, NForm, NFormItem, NCard } from 'naive-ui';
import { login } from '@/auth';

const router = useRouter();
const email = ref('');
const password = ref('');
const errorMessage = ref('');

const handleLogin = async () => {
  try {
    const response = await loginUser(email.value, password.value);

    if (response.data) {
      login(response.data); // Сохраняем пользователя через auth.js
      router.push('/'); // Переход на главную страницу
    }
  } catch (error) {
    errorMessage.value = 'Неверный email или пароль';
  }
};
</script>

<template>
  <div class="login-wrapper">
    <n-card title="Вход" bordered hoverable class="login-card">
      <n-form>
        <n-form-item label="Email">
          <n-input v-model:value="email" placeholder="Введите email" />
        </n-form-item>
        <n-form-item label="Пароль">
          <n-input v-model:value="password" type="password" placeholder="Введите пароль" />
        </n-form-item>
        <n-form-item class="login-button">
          <n-button type="primary" @click="handleLogin">Войти</n-button>
        </n-form-item>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
      </n-form>
    </n-card>
  </div>
</template>

<style>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh; /* Центрируем по всей высоте */
  position: absolute;
  width: 100%;
  top: 0;
  left: 0;
}

.login-card {
  width: 320px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}

.login-button {
  width: 100%;
  display: flex;
  justify-content: center;
}

.error {
  color: red;
  text-align: center;
}
</style>

