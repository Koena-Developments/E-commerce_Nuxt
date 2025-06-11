<template>
  <div class="auth-page-container">
    <div class="profile-card">
      <h1 class="card-title">My Profile Settings</h1>

      <div v-if="loadingProfile || status === 'loading'" class="loading-state">
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

      <form v-else-if="status === 'authenticated' && userProfile && userProfile.id" class="profile-form" @submit.prevent="saveProfileChanges">
        <div class="form-group">
          <label for="username" class="form-label">Username:</label>
          <input
            type="text"
            id="username"
            v-model="editableUserProfile.username"
            class="form-input"
            :disabled="!editMode"
            required
          />
        </div>

        <div class="form-group">
          <label for="email" class="form-label">Email:</label>
          <input
            type="email"
            id="email"
            v-model="editableUserProfile.email"
            class="form-input"
            :disabled="editMode"
            required
          />
        </div>

        <div class="form-group">
          <label for="password" class="form-label">New Password:</label>
          <input
            type="password"
            id="password"
            v-model="editableUserProfile.password"
            class="form-input"
            :disabled="!editMode"
            placeholder="Leave blank to keep current password"
            minlength="6"
          />
        </div>

        <div class="form-group">
          <label for="id" class="form-label">User ID:</label>
          <input
            type="text"
            id="id"
            v-model="editableUserProfile.id"
            class="form-input"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="createdAt" class="form-label">Member Since:</label>
          <input
            type="text"
            id="createdAt"
            :value="formatDate(editableUserProfile.createdAt)"
            class="form-input"
            disabled
          />
        </div>

        <button
          type="button"
          class="edit-button"
          @click="editMode ? saveProfileChanges() : toggleEditMode()"
          :disabled="isUpdating"
        >
          <span v-if="isUpdating">Saving...</span>
          <span v-else>{{ editMode ? 'Save Changes' : 'Edit Profile' }}</span>
        </button>

        <button
          v-if="editMode"
          type="button"
          class="cancel-button"
          @click="cancelEdit()"
          :disabled="isUpdating"
        >
          Cancel
        </button>
      </form>

      <div v-else class="loading-state">
        <p class="loading-text">Fetching profile details...</p>
        <div class="spinner"></div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'; 
import { useRuntimeConfig, navigateTo } from '#app'; 


// moving to pinia
const userProfile = ref({
  id: '',
  username: '',
  email: '',
  createdAt: '',
});
// moving to pinia

const editableUserProfile = ref({
  id: '',
  username: '',
  email: '',
  password: '',
  createdAt: '',
});
// moving to pinia

const profileError = ref(null);
const loadingProfile = ref(true);

const { data: authData, status } = useAuth(); 

const runtimeConfig = useRuntimeConfig();

const editMode = ref(false);
const isUpdating = ref(false);

const fetchUserProfile = async () => {
  loadingProfile.value = true; 
  profileError.value = null; 

  if (status.value === 'authenticated' && authData.value?.accessToken) {
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
        userProfile.value = { ...response.result };
        editableUserProfile.value = { ...response.result, password: '' };
        profileError.value = null;
      } else {
        const apiError = response?.error || 'Failed to load profile.';
        profileError.value = apiError;
        if (response?.statusCode === 401 || response?.statusCode === 403) {
          navigateTo('/Auth/login');
        }
      }
    } catch (error) {
      console.error('Profile fetch error:', error);
      let errorMessage = 'Failed to load profile.';
      const statusCode = error.statusCode || error.status;

      if (error.data?.error) {
        errorMessage = error.data.error;
      } else if (error.message) {
        errorMessage = error.message;
      }
      profileError.value = errorMessage;

      if (statusCode === 401 || statusCode === 403) {
        navigateTo('/Auth/login');
      }
    } finally {
      loadingProfile.value = false; 
    }
  } else {
    loadingProfile.value = false; 
    profileError.value = 'User not authenticated.';
  }
};

onMounted(fetchUserProfile); 

watch(status, (newStatus) => {
  if (newStatus === 'authenticated' && !userProfile.value.id) {
    fetchUserProfile();
  } else if (newStatus === 'unauthenticated') {
    userProfile.value = { id: '', username: '', email: '', createdAt: '' };
    editableUserProfile.value = { id: '', username: '', email: '', password: '', createdAt: '' };
    profileError.value = null;
    editMode.value = false;
    loadingProfile.value = false; 
  }
});


const toggleEditMode = () => {
  editMode.value = !editMode.value;
  if (editMode.value) {
    editableUserProfile.value = { ...userProfile.value, password: '' };
    profileError.value = null;
  }
};

const cancelEdit = () => {
  editMode.value = false;
  editableUserProfile.value = { ...userProfile.value, password: '' };
  profileError.value = null;
};

const saveProfileChanges = async () => {
  if (isUpdating.value) return;

  profileError.value = null;

  if (!editableUserProfile.value.username) {
    profileError.value = 'Username is required.';
    return;
  }
  if (!editableUserProfile.value.email) {
    profileError.value = 'Email is required.';
    return;
  }

  const payload = {
    username: editableUserProfile.value.username,
    email: editableUserProfile.value.email,
  };

  if (editableUserProfile.value.password) {
    payload.password = editableUserProfile.value.password;
  }

  isUpdating.value = true;

  try {
    const response = await $fetch('/User/profile', {
      method: 'PUT',
      baseURL: runtimeConfig.public.apiBaseUrl,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authData.value.accessToken}`,
      },
      body: payload,
    });

    if (response && response.status === true) {
      if (response.result) {
        userProfile.value = { ...response.result };
        editableUserProfile.value = { ...response.result, password: '' };
      }

      if (authData.value?.refresh) {
        await authData.value.refresh();
      }

      editMode.value = false;
      profileError.value = null;
    } else {
      profileError.value = response?.error || 'Failed to save profile changes.';
    }

  } catch (error) {
    console.error('Profile update error:', error);

    let errorMessage = 'Failed to update profile.';
    const statusCode = error.statusCode || error.status;

    if (error.data?.error) {
      errorMessage = error.data.error;
    } else if (error.message) {
      errorMessage = error.message;
    }

    profileError.value = errorMessage;

    if (statusCode === 401 || statusCode === 403) {
      navigateTo('/Auth/login');
    }
  } finally {
    isUpdating.value = false;
  }
};

const formatDate = (dateString) => {
  if (!dateString) return '';
  try {
    const options = {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    };
    return new Date(dateString).toLocaleDateString(undefined, options);
  } catch (error) {
    return dateString;
  }
};
</script>

<style scoped>
.auth-page-container {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  min-height: 100vh;
  padding: 1rem;
  background-color: #f3f4f6;
  font-family: "Inter", sans-serif;
  box-sizing: border-box;
}

.profile-card {
  max-width: 450px;
  width: 100%;
  background-color: #ffffff;
  border-radius: 8px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  padding: 2rem;
  margin-top: 2rem;
}

.card-title {
  font-size: 2rem;
  font-weight: bold;
  color: #1f2937;
  margin-bottom: 1.5rem;
  text-align: center;
}

.loading-state, .unauthenticated-state {
  text-align: center;
  padding: 2rem 0;
}

.loading-text, .unauthenticated-message {
  color: #4b5563;
  font-size: 1rem;
  margin-bottom: 1rem;
}

.spinner {
  border: 4px solid rgba(0, 0, 0, 0.1);
  border-left-color: #3b82f6;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 1rem auto;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-alert {
  background-color: #fee2e2;
  border: 1px solid #ef4444;
  color: #b91c1c;
  padding: 1rem;
  border-radius: 6px;
  margin-bottom: 1.5rem;
}

.alert-strong {
  font-weight: bold;
  display: block;
  margin-bottom: 0.25rem;
}

.alert-message {
  display: block;
}

.login-button {
  display: inline-block;
  background-color: #2563eb;
  color: #ffffff;
  font-weight: bold;
  padding: 0.75rem 1.5rem;
  border-radius: 9999px;
  text-decoration: none;
  transition: background-color 0.3s ease;
  margin-top: 1rem;
}

.login-button:hover {
  background-color: #1d4ed8;
}

.profile-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  margin-bottom: 0;
}

.form-label {
  display: block;
  color: #374151;
  font-size: 0.875rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.form-input {
  display: block;
  width: 100%;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  line-height: 1.5;
  color: #4b5563;
  background-color: #ffffff;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  transition: border-color 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.5);
}

.form-input[disabled] {
  background-color: #f9fafb;
  cursor: not-allowed;
  color: #6b7280;
  border-color: #e5e7eb;
}

.edit-button {
  width: 100%;
  background-color: #4f46e5;
  color: #ffffff;
  font-weight: bold;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}

.edit-button:hover:not(:disabled) {
  background-color: #4338ca;
  box-shadow: 0 6px 8px -2px rgba(0, 0, 0, 0.15), 0 3px 5px -1px rgba(0, 0, 0, 0.08);
}

.edit-button:focus {
  outline: none;
  box-shadow: 0 0 0 4px rgba(79, 70, 229, 0.5);
}

.edit-button:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.cancel-button {
  width: 100%;
  background-color: #ef4444;
  color: #ffffff;
  font-weight: bold;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  margin-top: 0.5rem;
}

.cancel-button:hover:not(:disabled) {
  background-color: #dc2626;
  box-shadow: 0 6px 8px -2px rgba(0, 0, 0, 0.15), 0 3px 5px -1px rgba(0, 0, 0, 0.08);
}

.cancel-button:focus {
  outline: none;
  box-shadow: 0 0 0 4px rgba(239, 68, 68, 0.5);
}

.cancel-button:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}
</style>