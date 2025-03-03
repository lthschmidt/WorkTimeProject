import api from './api';

// Список проектов
export function getProjects() {
    return api.get('api/Project');
}

// Данные о проете по его id
export function getProject(id) {
    return api.get(`api/Project/${id}`);
}

// Создать новый проект
export function createProject(data) {
    return api.post('api/Project', data);
}

// Обновить данные о проекте с id
export function updateProject(id, data) {
    return api.put(`api/Project/${id}`, data);
}

// Удалить проект по его id
export function deleteProject(id) {
    return api.delete(`api/Project/${id}`);
}
