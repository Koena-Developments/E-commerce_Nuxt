<template>
  <div class="auth-container">
    <h2>Login</h2>
    <form @submit.prevent="handleLogin" class="auth-form">
      <div class="form-group">
        <label for="username">Username or Email:</label>
        <input type="text" id="username" v-model="loginData.username" required />
      </div>
      <div class="form-group">
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="loginData.password" required />
      </div>
      <button type="submit" :disabled="loading">
        {{ loading ? 'Logging In...' : 'Login' }}
      </button>

      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>

      <p class="auth-link">
        Don't have an account? <NuxtLink to="/auth/register">Register here</NuxtLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
const router = useRouter();

const loginData = ref({
  username: '',
  password: '',
});

const loading = ref(false);
const errorMessage = ref('');

const successMessage = ref('');

const {SignIn} = useAuth()
const handleLogin = async () => {
  loading.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    const { data, error, pending, status } = await useFetch('http://localhost:5000/api/Auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(loginData.value),
    });

    if (status.value === 'success' && !error.value) {
      successMessage.value = data.value.Message || 'Login successful!';
      console.log('Login successful! Token:', data.value.Token);
      router.push('/');
    } else {
      errorMessage.value = error.value?.data?.Message || 'Login failed. Please try again.';
      console.error('Login error:', error.value?.data?.Message || error.value);
    }
  } catch (err) {
    errorMessage.value = 'An unexpected error occurred. Please try again later.';
    console.error('Network or unexpected error:', err);
  } finally {
    loading.value = false;
  }
};
</script>
<style scoped>
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

label {
  display: block;
  margin-bottom: 8px;
  font-weight: bold;
  color: #555;
}

input[type="text"],
input[type="password"] {
  width: calc(100% - 20px);
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

button {
  width: 100%;
  padding: 12px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 18px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover:not(:disabled) {
  background-color: #0056b3;
}

button:disabled {
  background-color: #a0c9ff;
  cursor: not-allowed;
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