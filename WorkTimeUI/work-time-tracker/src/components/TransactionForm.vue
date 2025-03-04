<script setup>
import { defineProps, defineEmits, ref, watch } from 'vue';
import { createTransaction } from '@/services/transactions';
import { NInput, NSpace, useMessage, NForm, NFormItem } from 'naive-ui';
import { user } from '@/auth.js';
import { useRoute } from 'vue-router';

const props = defineProps({ transaction: Object });
const route = useRoute();

const emit = defineEmits(['update', 'close']);
const message = useMessage();

const editedTransaction = ref(
    route.params.taskID ? { taskId: Number(route.params.taskID) } : { taskId: null }
);



// Обновлять editedTransaction при изменении props.transaction
watch(() => props.transaction, (newTransaction) => {
    editedTransaction.value = { ...newTransaction };
});

const saveTransaction = async () => {
    editedTransaction.value.userId = user?.value.id;
    try {
        await createTransaction(editedTransaction.value);
        emit('update'); // Сообщаем родителю, что обновили
        emit('close');
    } catch (error) {
        message.error(error.response.data);
    }
};

</script>

<template>
    <n-form label-placement="left"  label-width="auto" >
        <n-space vertical justify="center">
            <n-form-item label="Имя пользователя">
                <n-input :value="user.name" type="text" placeholder="Имя пользователя"
                    @update:value="(newValue) => editedTransaction.name = newValue" />
            </n-form-item>
            <n-form-item label="Задача">
                <n-input v-model="editedTransaction.taskId" :default-value="editedTransaction.taskId" type="text"
                    placeholder="Задача" @update:value="(newValue) => editedTransaction.taskId = newValue" />
            </n-form-item>
            <n-form-item label="Описание">
                <n-input v-model="editedTransaction.description" :default-value="editedTransaction.description" type="textarea"
                    placeholder="Описание" @update:value="(newValue) => editedTransaction.description = newValue" />
            </n-form-item>
            <n-form-item label="Часы">
                <n-input v-model="editedTransaction.hours" :default-value="editedTransaction.hours" type="text"
                    placeholder="Часы" @update:value="(newValue) => editedTransaction.hours = newValue" />
            </n-form-item>
            <n-space horizontal justify="center">
                <n-form-item>
                <n-button size="small" type="success" @click="saveTransaction">
                    Создать
                </n-button>
            </n-form-item>
            <n-form-item>
                <n-button size="small" type="error" @click="$emit('close')">Закрыть</n-button>
            </n-form-item>
            </n-space>
            
        </n-space>
    </n-form>

</template>
