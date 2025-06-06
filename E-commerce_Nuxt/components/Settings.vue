<template>
</template>
<script setup>
import {ref, onMounted, watch} from 'vue';
import {useAuth, useRuntimeConfig} from '#app';

const userProfile = ref({
    id: '',
    username: '',
    email: '',
    createdAt: '',
});

const profileError = ref(null);
const {data: authData, status} = useAuth();

const runtimeConfig= useRuntimeConfig();

const fetchUserProfile = async () => {
    profileError.value = null;

    if (status.value === 'authenticated' && authData.value.accessToken) {
        try {
            const response = await $fetch('/User/profile', {
                method: 'GET',
                baseURL: runtimeConfig.public.apiBaseUrl,
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authData.value.accessToken}`,
                },
            });

            if (response && response.status === true && response.result) {
                userProfile.value = {
                    id: response.result.id,
                    username: response.result.username,
                    email: response.result.email,
                };
            } else {
                profileError.value = response?.error || 'failed to load profile';
                console.error('API Error:', response.error);
            }
        } catch (error) {
            profileError.value = `error fetching profile: ${error.statusCode || error.message}`;
            console.error('Network Error:', error);
            if (error.statusCode === 401 || error.statusCode === 403) {
                navigateTo('/Auth/login');
            }
        }
    } else if (status.value === 'unauthenticated') {
        profileError.value = 'Please log in to view profile.';
    }
};

const formatDate = (dateString) => {
    if (!dateString) return '';
    const options = { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleDateString(undefined, options);
};

onMounted(() => {
    if (status.value === "authenticated") {
        fetchUserProfile();
    }
});

watch(status, (newStatus) => {
    if (newStatus === 'authenticated') {
        userProfile.value = { id: '', username: '', email: '', createdAt: '' };
        profileError.value = 'please log in to see profile.';
    }
});




</script>