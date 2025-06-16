<template>
  <div class="checkout-card">
    <h1 class="checkout-title">Secure Checkout</h1>

    <!-- Empty Cart State -->
    <div v-if="cart.length === 0 && !isProcessingPayment" class="message-state">
      <p class="empty-cart-text">Your cart is empty!</p>
    </div>
    <div v-else-if="cart.length === 0" class="message-state">
      <p class="empty-cart-text">Your cart is empty!</p>
      <p class="message-text">Please add items to your cart before proceeding to checkout.</p>
      <NuxtLink to="/" class="continue-shopping-button">
        Continue Shopping
      </NuxtLink>
    </div>

    <!-- Checkout Form and Order Summary -->
    <div v-else class="checkout-grid">
      <!-- Shipping Info Form -->
      <div class="shipping-info-form-section">
        <h2 class="section-title">Shipping Information</h2>
        <form @submit.prevent="proceedToPayment" class="form-spacing">
          <div class="form">
            <div>
              <label for="fullName" class="form-label">Full Name</label>
              <input
                type="text"
                id="fullName"
                v-model="checkoutDetails.fullName"
                class="form-input"
                required
              />
            </div>
            <div>
              <label for="phoneNumber" class="form-label">Phone Number</label>
              <input
                type="tel"
                id="phoneNumber"
                v-model="checkoutDetails.phoneNumber"
                class="form-input"
                placeholder="+1234567890"
                required
              />
            </div>
          </div>

          <div>
            <label for="addressLine1" class="form-label">Address Line 1</label>
            <input
              type="text"
              id="addressLine1"
              v-model="checkoutDetails.addressLine1"
              class="form-input"
              required
            />
          </div>

          <div>
            <label for="addressLine2" class="form-label">Address Line 2 (Optional)</label>
            <input
              type="text"
              id="addressLine2"
              v-model="checkoutDetails.addressLine2"
              class="form-input"
            />
          </div>

          <div class="form">
            <div>
              <label for="city" class="form-label">City</label>
              <input
                type="text"
                id="city"
                v-model="checkoutDetails.city"
                class="form-input"
                required
              />
            </div>
            <div>
              <label for="state" class="form-label">State / Province</label>
              <input
                type="text"
                id="state"
                v-model="checkoutDetails.state"
                class="form-input"
                required
              />
            </div>
            <div>
              <label for="postalCode" class="form-label">Postal Code</label>
              <input
                type="text"
                id="postalCode"
                v-model="checkoutDetails.postalCode"
                class="form-input"
                required
              />
            </div>
          </div>

          <div>
            <label for="country" class="form-label">Country</label>
            <input
              type="text"
              id="country"
              v-model="checkoutDetails.country"
              class="form-input"
              required
            />
          </div>

          <p v-if="errorMessage" class="error-message-box">{{ errorMessage }}</p>

          <button
            type="submit"
            :disabled="isProcessingPayment || !isFormValid"
            class="proceed-button"
          >
            <span v-if="isProcessingPayment" class="button-content">
              <svg class="spinner-button" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Processing Payment...
            </span>
            <span v-else>Proceed to Payment</span>
          </button>
        </form>
      </div>

      <!-- Order Summary -->
      <div class="order-summary-section">
        <h2 class="section-title">Order Summary</h2>
        <div class="order-items-list">
          <div v-for="item in cart" :key="item.id" class="order-item">
            <div class="order-item-details">
              <img
                :src="item.image || 'https://placehold.co/60x60/a8a29e/ffffff?text=Product'"
                alt="Product image"
                class="product-image"
              />
              <div>
                <h3 class="product-name">{{ item.name }}</h3>
                <p class="product-quantity">Quantity: {{ item.quantity }}</p>
              </div>
            </div>
            <p class="product-price">R {{ item.price * item.quantity }}</p>
          </div>
        </div>
        <div class="order-total-section">
          <div class="order-total-row">
            <span>Total:</span>
            <span>R {{ cartTotal }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useMyProductStore } from '~/stores/productStore';
import { useUserStore } from '~/stores/UserStore';
import { storeToRefs } from 'pinia'

const cartStore = useMyProductStore();
const userStore = useUserStore();
const { cart, cartTotal } = storeToRefs(cartStore);
const { userProfile } = storeToRefs(userStore);
const { status: authStatus, data: authData } = useAuth();

const isProcessingPayment = ref(false);
const errorMessage = ref('');

const checkoutDetails = ref({
  fullName: '',
  phoneNumber: '',
  addressLine1: '',
  addressLine2: '',
  city: '',
  state: '',
  postalCode: '',
  country: '',
});

// Validate required fields
const isFormValid = computed(() => {
  const d = checkoutDetails.value;
  return d.fullName && d.phoneNumber && d.addressLine1 &&
         d.city && d.state && d.postalCode && d.country;
});

// Prefill full name if user is authenticated
onMounted(() => {
  if (authStatus.value === 'authenticated' && authData.value?.user) {
    if (userProfile.value.username) {
      checkoutDetails.value.fullName = userProfile.value.username;
    } else if (authData.value.user.name) {
      checkoutDetails.value.fullName = authData.value.user.name;
    }
  }
});
async function proceedToPayment() {
  if (!cart.value.length) {
    errorMessage.value = "Your cart is empty.";
    return;
  }

  isProcessingPayment.value = true;
  errorMessage.value = "";

  try {
    const cartItems = cart.value.map(item => ({
      productId: Number(item.id),
      quantity: Number(item.quantity),
      price: parseFloat(item.price), 
      name: item.title || '',
      imageUrl: item.image || '',
    }));

const response = await $fetch('/api/checkout/create-checkout-session', {
  baseURL: 'http://localhost:5000',
  method: 'POST',
  body: cartItems, 
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${authData.value?.accessToken}`
  }
});

    console.log(cartItems)
    if (response.status && response.result?.checkoutUrl) {
      window.location.href = response.result.checkoutUrl;
    } else {
      errorMessage.value = response.error || "Failed to create checkout session.";
    }

  } catch (err) {
    console.error("Checkout Error:", err?.data || err);
    errorMessage.value = err?.data?.message || "Something went wrong. Please try again.";
  } finally {
    isProcessingPayment.value = false;
  }
}

</script>


<style scoped>
.checkout-card {
  max-width: 1000px;
  margin: 2rem auto;
  padding: 2rem;
  background-color: #ffffff;
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.checkout-title {
  font-size: 2rem;
  font-weight: bold;
  color: #2c3e50;
  margin-bottom: 1.5rem;
  text-align: center;
}

.message-state {
  text-align: center;
  padding: 2rem;
}

.empty-cart-text {
  font-size: 1.25rem;
  font-weight: 500;
  color: #c0392b;
}

.message-text {
  color: #7f8c8d;
  margin-top: 0.5rem;
}

.continue-shopping-button {
  margin-top: 1rem;
  padding: 0.6rem 1.2rem;
  background-color: #3498db;
  color: #fff;
  border-radius: 8px;
  display: inline-block;
  text-decoration: none;
  font-weight: 500;
}

.checkout-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #34495e;
}

.form-spacing {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.form-label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
  color: #2c3e50;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #dcdcdc;
  border-radius: 8px;
  font-size: 1rem;
  outline: none;
  transition: border-color 0.2s;
}

.form-input:focus {
  border-color: #3498db;
}

.proceed-button {
  margin-top: 1rem;
  width: 100%;
  background-color: #27ae60;
  color: white;
  padding: 0.75rem;
  font-size: 1rem;
  font-weight: bold;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.proceed-button:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

.proceed-button:hover:not(:disabled) {
  background-color: #219150;
}

.spinner-button {
  width: 20px;
  height: 20px;
  margin-right: 0.5rem;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.order-summary-section {
  background-color: #f8f9fa;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #e0e0e0;
}

.order-items-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.order-item-details {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.product-image {
  width: 60px;
  height: 60px;
  border-radius: 8px;
  object-fit: cover;
}

.product-name {
  font-weight: 600;
  color: #2c3e50;
}

.product-quantity {
  font-size: 0.9rem;
  color: #7f8c8d;
}

.product-price {
  font-weight: 500;
  color: #2c3e50;
}

.order-total-section {
  border-top: 1px solid #ccc;
  margin-top: 1rem;
  padding-top: 1rem;
  display: flex;
  justify-content: space-between;
  font-size: 1.1rem;
  font-weight: bold;
  color: #34495e;
}

.error-message-box {
  background-color: #fbeaea;
  color: #c0392b;
  padding: 0.75rem;
  border: 1px solid #e74c3c;
  border-radius: 6px;
  margin-top: 1rem;
}
</style>
