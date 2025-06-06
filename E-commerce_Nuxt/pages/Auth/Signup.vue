<template>
  <div class="auth-container">
    <h2>Register</h2>
    <form @submit.prevent="handleRegister" class="auth-form">
      <div class="form-group">
        <label for="reg-username">Username:</label>
        <input
          type="text"
          id="reg-username"
          v-model="registerData.username"
          required
          :disabled="loading"
        />
      </div>
      <div class="form-group">
        <label for="reg-email">Email:</label>
        <input
          type="email"
          id="reg-email"
          v-model="registerData.email"
          required
          :disabled="loading"
        />
      </div>
      <div class="form-group">
        <label for="reg-password">Password:</label>
        <input
          type="password"
          id="reg-password"
          v-model="registerData.password"
          required
          :disabled="loading"
        />
      </div>
      <button type="submit" :disabled="loading">
        {{ loading ? 'Registering...' : 'Register' }}
      </button>

      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>

      <p class="auth-link">
        Already have an account? <NuxtLink to="/Auth/login">Login here</NuxtLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { navigateTo, useRuntimeConfig } from '#app';

const runtimeConfig = useRuntimeConfig();

const registerData = ref({
  username: '',
  email: '',
  password: '',
});

const loading = ref(false);
const errorMessage = ref('');
const successMessage = ref('');

const handleRegister = async () => {
  loading.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    const registerResponse = await $fetch('/Auth/register', {
      method: 'POST',
      baseURL: runtimeConfig.public.apiBaseUrl,
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(registerData.value),
    });

    if (registerResponse && registerResponse.status === true) {
      successMessage.value = registerResponse.result?.message || 'Registration successful! You can now log in.';
      console.log('Registration successful:', registerResponse.result?.message);
 console.log('Auth Status BEFORE navigateTo:', useAuth().status.value);
      setTimeout(() => {
        navigateTo('/Auth/Login');
      }, 1500);

    } else {
      errorMessage.value = registerResponse?.error || 'Registration failed. Please try again.';
      console.error('Registration API error:', registerResponse?.error);
    }
  } catch (error) {
    errorMessage.value = 'An unexpected error occurred. Please try again later.';
    console.error('Network or unexpected error during registration:', error);

    if (error?.data?.error) {
      errorMessage.value = error.data.error;
      console.error('Backend error details:', error.data.error);
    }
  } finally {
    loading.value = false;
  }
};

definePageMeta({
  auth: false,
  layout: 'auth'
});
</script>

<style scoped>
.auth-container {
  max-width: 400px;
  margin: 2rem auto;
  padding: 2rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: #f9f9f9;
}

.auth-container h2 {
  text-align: center;
  margin-bottom: 1.5rem;
  color: #333;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #555;
}

.form-group input {
  padding: 0.75rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.3s ease;
}

.form-group input:focus {
  outline: none;
  border-color: #007bff;
}

.form-group input:disabled {
  background-color: #f8f9fa;
  cursor: not-allowed;
}

button[type="submit"] {
  padding: 0.75rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button[type="submit"]:hover:not(:disabled) {
  background-color: #0056b3;
}

button[type="submit"]:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  color: #dc3545;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  padding: 0.75rem;
  border-radius: 4px;
  margin: 0;
  font-size: 0.9rem;
}

.success-message {
  color: #155724;
  background-color: #d4edda;
  border: 1px solid #c3e6cb;
  padding: 0.75rem;
  border-radius: 4px;
  margin: 0;
  font-size: 0.9rem;
}

.auth-link {
  text-align: center;
  margin-top: 1rem;
  color: #666;
}

.auth-link a {
  color: #007bff;
  text-decoration: none;
}

.auth-link a:hover {
  text-decoration: underline;
}
</style>