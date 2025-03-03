import api from './api';

// Получить все задачи или только по проекту
export function getTasks(projectId) {
    return api.get('api/Task',
        {params: {projectId}}
    );
}

// Получить данные о задаче с id
export function getTask(id) {
    return api.get(`api/Task/${id}`);
}

// Создать задачу
export function createTask(data) {
    return api.post('api/Task', data);
}

// Обновить данные о задаче с id
export function updateTask(id, data) {
    return api.put(`api/Task/${id}`, data);
}

// Удалить задачу с id
export function deleteTask(id) {
    return api.delete(`api/Task/${id}`);
}
