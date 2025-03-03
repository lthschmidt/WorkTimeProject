import { ref } from 'vue';

const user = ref(null);

const fetchUser = () => {
  const savedUser = localStorage.getItem('user');
  user.value = savedUser ? JSON.parse(savedUser) : null;
};

const login = (userData) => {
  localStorage.setItem('user', JSON.stringify(userData));
  user.value = userData;
};

const logout = () => {
  localStorage.removeItem('user');
  user.value = null;
};

fetchUser(); // Загружаем пользователя при запуске

export { user, fetchUser, login, logout };
