<template>
  <div class="checkout-container">
    <div class="checkout-card">
      <h1 class="checkout-title">Secure Checkout</h1>
     
      <!-- Loading/Error State for Cart -->
      <div v-if="cartVisible" class="message-state">
        <p class="message-text">Loading your cart...</p>
        <div class="spinner"></div>
      </div>
      <div v-else-if="cart === 0" class="message-state">
        <p class="empty-cart-text">Your cart is empty!</p>
        <p class="message-text">Please add items to your cart before proceeding to checkout.</p>
        <NuxtLink to="/" class="continue-shopping-button">
          Continue Shopping
        </NuxtLink>
      </div>
      <div v-else class="checkout-grid">

        <!-- Shipping and Contact Information Form -->
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

            <p v-if="errorMessage" class="error-message-box">
              {{ errorMessage }}
            </p>

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
                <img :src="item.imageUrl || 'https://placehold.co/60x60/a8a29e/ffffff?text=Product'" alt="Product image" class="product-image" />
                <div>
                  <h3 class="product-name">{{ item.name }}</h3>
                  <p class="product-quantity">Quantity: {{ item.quantity }}</p>
                </div>
              </div>
              <p class="product-price">R {{cartTotal }}</p>
            </div>
          </div>
          <div class="order-total-section">
            <div class="order-total-row">
              <span>Total:</span>
              <span>R {{ cart }}</span>
            </div>
            </div>

          </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import {useMyProductStore} from '~/stores/productStore';
import {useUserStore} from '~/stores/UserStore';

// initialize pinia store
const cartStore = useMyProductStore();
const userStore = useUserStore();


const {cart, cartTotal, cartVisible} = storeToRefs(cartStore);
const {userProfile} = storeToRefs(userStore);
const { status: authStatus, data: authData } = useAuth();



// const cartStore = ref({
//   isCartLoaded: true,
//   cartItems: [{ id: 1, name: 'Sample Item', price: 10 }]
// })

const checkoutDetails = ref({
  fullName: '',
  phoneNumber: '',
  addressLine1: '',
  addressLine2: '',
  city: '',
  state: '',
  postalCode: '',
  country: '',
})

onMounted(() => {
  if (authStatus.value === 'authenticated' && authData.value?.user) {
    if (userProfile.username) {
      checkoutDetails.value.fullName = userProfile.username;
    } else if (authData.value.user.name) {
      checkoutDetails.value.fullName = authData.value.user.name;
    }
  }
});
function proceedToPayment() {
  alert('Proceeding to payment with: ' + JSON.stringify(checkoutDetails.value))
}


</script>