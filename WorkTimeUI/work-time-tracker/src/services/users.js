import api from './api';

// Получить данные о пользователе с id
export function getUser(id) {
    return api.get(`api/User/${id}`);
}

// Вернуть данные о пользователе
export function loginUser(email, password) {
    return api.post(`api/User/login`, {
        email: email,
        password: password
    }
    );
}