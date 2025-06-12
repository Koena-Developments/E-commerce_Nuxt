import { defineStore } from 'pinia';
import { ref, watch } from 'vue';
import { useRuntimeConfig, navigateTo } from '#app';
import { useAuth } from '#imports';

export const useUserStore = defineStore('user', () => {
  // State
  const profileError = ref(null);
  const loadingProfile = ref(false); 
  const isUpdating = ref(false);
  const editMode = ref(false);

  const userProfile = ref({
    id: null,
    username: '',
    email: '',
    createdAt: '',
  });

  const editableUserProfile = ref({
    id: null,
    username: '',
    email: '',
    password: '',
    createdAt: '',
  });

  // Composables
  const { data: authData, status, refresh } = useAuth();
  const runtimeConfig = useRuntimeConfig();

  // Actions
  const fetchUserProfile = async () => {
    loadingProfile.value = true;
    profileError.value = null;

    if (status.value === 'authenticated' && authData.value?.accessToken) {
      try {
        const response = await $fetch(`/User/profile`, {
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
          const apiError = response?.error || 'Failed to load profile. Unknown API response.';
          profileError.value = apiError;
          if (response?.statusCode === 401 || response?.statusCode === 403 || (response && response.status === false && (response.error?.includes("401") || response.error?.includes("403")))) {
             navigateTo('/Auth/login');
          }
        }
      } catch (error) {
        console.error('Profile fetch error:', error);
        let errorMessage = 'Failed to load profile due to network or server error.';
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
      if (status.value === 'unauthenticated') {
      } else {
          profileError.value = 'User not authenticated or authentication data is incomplete.';
      }
      userProfile.value = { id: null, username: '', email: '', createdAt: '' };
      editableUserProfile.value = { id: null, username: '', email: '', password: '', createdAt: '' };
    }
  };

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

    if (status.value !== 'authenticated' || !authData.value?.accessToken) {
      profileError.value = 'Not authenticated. Please log in.';
      isUpdating.value = false;
      navigateTo('/Auth/login');
      return;
    }

    try {
      const response = await $fetch(`/User/profile`, {
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
        } else {
          await fetchUserProfile();
        }

        if (refresh) {
            await refresh();
        } else if (authData.value && authData.value.user) {
          authData.value.user.username = userProfile.value.username;
          authData.value.user.email = userProfile.value.email;
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

  // Watch auth status changes to trigger fetch/clear
  watch(status, (newStatus) => {
    if (newStatus === 'authenticated' && authData.value?.accessToken) {
      fetchUserProfile();
    } else if (newStatus === 'unauthenticated') {
      userProfile.value = { id: null, username: '', email: '', createdAt: '' };
      editableUserProfile.value = { id: null, username: '', email: '', password: '', createdAt: '' };
      profileError.value = null;
      loadingProfile.value = false;
      editMode.value = false;
      isUpdating.value = false;
    }
  }, { immediate: true });

  // Return state and actions
  return{
    userProfile,
    editableUserProfile,
    profileError,
    loadingProfile,
    isUpdating,
    editMode,
    fetchUserProfile,
    toggleEditMode,
    cancelEdit,
    saveProfileChanges
  }
});
