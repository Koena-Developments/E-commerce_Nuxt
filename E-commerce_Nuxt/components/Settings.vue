<template>
  <div class="auth-page-container">
    <div class="profile-card">
      <h1 class="card-title">My Profile Settings</h1>

      <div v-if="status === 'loading'" class="loading-state">
        <p class="loading-text">Loading profile data...</p>
        <div class="spinner"></div>
      </div>

      <div v-else-if="profileError" class="error-alert" role="alert">
        <strong class="alert-strong">Error!</strong>
        <span class="alert-message">{{ profileError }}</span>
      </div>

      <div v-else-if="status === 'unauthenticated'" class="unauthenticated-state">
        <p class="unauthenticated-message">You must be logged in to view your profile settings.</p>
        <NuxtLink to="/Auth/login" class="login-button">
          Login Now
        </NuxtLink>
      </div>

      <form v-else class="profile-form">
        <div class="form-group">
          <label for="username" class="form-label">Username:</label>
          <input
            type="text"
            id="username"
            v-model="userProfile.username"
            class="form-input"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="email" class="form-label">Email:</label>
          <input
            type="email"
            id="email"
            v-model="userProfile.email"
            class="form-input"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="id" class="form-label">User ID:</label>
          <input
            type="text"
            id="id"
            v-model="userProfile.id"
            class="form-input"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="createdAt" class="form-label">Member Since:</label>
          <input
            type="text"
            id="createdAt"
            :value="formatDate(userProfile.createdAt)"
            class="form-input"
            disabled
          />
        </div>

        <button
          type="button"
          class="edit-button"
          @click="alert('To edit, we need a backend PUT/PATCH endpoint!')"
        >
          Edit Profile (Feature Coming Soon!)
        </button>
      </form>
    </div>
  </div>
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