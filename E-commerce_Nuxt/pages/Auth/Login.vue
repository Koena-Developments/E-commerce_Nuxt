<template>
  <div class="auth-container">
    <h2>Login</h2>
    <form @submit.prevent="handleLogin" class="auth-form">
      <div class="form-group">
        <label for="email">Email:</label>
        <input 
          type="email" 
          id="email" 
          v-model="loginData.email" 
          required 
          :disabled="loading"
        />
      </div>
      <div class="form-group">
        <label for="password">Password:</label>
        <input 
          type="password" 
          id="password" 
          v-model="loginData.password" 
          required 
          :disabled="loading"
        />
      </div>
      <button type="submit" :disabled="loading">
        {{ loading ? 'Logging In...' : 'Login' }}
      </button>
      
      
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>
      
      <p class="auth-link">
        Don't have an account? <NuxtLink to="/Auth/Signup">Register here</NuxtLink>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const { signIn, status } = useAuth()
const loginData = ref({
  email: '',
  password: '',
})

const loading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const handleLogin = async () => {
  loading.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const result = await signIn('credentials', {
      email: loginData.value.email,
      password: loginData.value.password,
      redirect: false
    })

    if (result?.error) {
      errorMessage.value = 'Invalid email or password. Please try again.'
      console.error('Login error:', result.error)
    }

    else if (result?.ok && status.value == "authenticated") {
      successMessage.value = 'Login successful!'
      console.log('Login successful!')
  
      setTimeout(async () => {
        await navigateTo('/')
      }, 500)
    } else {
      errorMessage.value = 'Login failed. Please check your credentials.'
    }
  } catch (err) {
    errorMessage.value = 'An unexpected error occurred. Please try again later.'
    console.error('Network or unexpected error:', err)
  } finally {
    loading.value = false
  }
}


definePageMeta({
  auth: false, 
})
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