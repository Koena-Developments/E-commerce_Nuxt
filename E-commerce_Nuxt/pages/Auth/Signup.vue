<template>
  <div class="auth-container">
    <h2>Register</h2>
    <form @submit.prevent="handleRegister" class="auth-form">
      <div class="form-group">
        <label for="reg-username">Username:</label>
        <input type="text" id="reg-username" v-model="registerData.username" required />
      </div>
      <div class="form-group">
        <label for="reg-email">Email:</label>
        <input type="email" id="reg-email" v-model="registerData.email" required />
      </div>
      <div class="form-group">
        <label for="reg-password">Password:</label>
        <input type="password" id="reg-password" v-model="registerData.password" required />
      </div>
      <button type="submit" :disabled="loading">
        {{ loading ? 'Registering...' : 'Register' }}
      </button>

      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>

      <p class="auth-link">
        Already have an account? <NuxtLink to="/auth/login">Login here</NuxtLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

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
    // --- API Call Placeholder ---
    const response = await fetch('http://localhost:5000/api/Auth/register', { // Replace with your actual API URL/proxy
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(registerData.value),
    });

    const data = await response.json();

    if (response.ok) {
      successMessage.value = data.Message || 'Registration successful!';
      setTimeout(() => {
        router.push('/auth/login');
      }, 2000);
    } else {
      errorMessage.value = data.Message || 'Registration failed. Please try again.';
      console.error('Registration error:', data.Message);
    }
  } catch (error) {
    errorMessage.value = 'An unexpected error occurred. Please try again later.';
    console.error('Network or unexpected error:', error);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
/* @import url('./login.vue');  */

.auth-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 80vh;
  padding: 20px;
  background-color: #f0f2f5;
}

.auth-form {
  background: #ffffff;
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
}

h2 {
  text-align: center;
  margin-bottom: 25px;
  color: #333;
}

.form-group {
  margin-bottom: 20px;
}

.error-message {
  color: #dc3545;
  margin-top: 15px;
  text-align: center;
}

.success-message {
  color: #28a745;
  margin-top: 15px;
  text-align: center;
}

.auth-link {
  margin-top: 20px;
  text-align: center;
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