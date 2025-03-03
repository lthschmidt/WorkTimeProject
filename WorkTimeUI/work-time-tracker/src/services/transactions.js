import api from './api';

// Получить проводки по критериям
export function getTransactions(taskId, userId, date) {
    return api.get('api/Transaction', {
        params: {
            ...(taskId && { taskId }),  // Добавляем taskId, если он есть
            ...(userId && { userId }),  // Добавляем userId, если он есть
            ...(date && { date })       // Добавляем date, если он есть
        }
    });
}

// Создать проводку
export function createTransaction(data) {
    return api.post('api/Transaction', data);
}

// Удалить проводку с id
export function deleteTransaction(id) {
    return api.delete(`api/Transaction/${id}`);
}
