<template>
  <div v-if="visible" class="cart-sidebar">
    <div class="cart-header">
      <h3>Your Cart</h3>
      <button class="close-btn" @click="$emit('close-cart')">✖</button>
    </div>

    <div class="cart-body">
      <div v-for="item in cart" :key="item.id" class="cart-item">
        <img :src="item.image" class="item-image" />
        <div class="item-info">
          <p class="item-title">{{ item.title }}</p>
          <div class="quantity-controls">
            <button @click="$emit('decrease', item)">-</button>
            <span>{{ item.quantity }}</span>
            <button @click="$emit('increase', item)">+</button>
          </div>
        </div>
      </div>
    </div>

    <div class="cart-footer">
      <p class="total">Total: R{{ total }}</p>
      <button class="checkout-btn">Checkout</button>
    </div>
  </div>
</template>

<script setup>
defineProps({
  cart: Array,
  visible: Boolean,
  total: Number
})

defineEmits(['close-cart', 'increase', 'decrease'])
</script>

<style scoped>
.cart-sidebar {
  position: fixed;
  top: 0;
  right: 0;
  width: 350px;
  height: 100%;
  background: white;
  border-left: 2px solid #ddd;
  box-shadow: -4px 0 8px rgba(0, 0, 0, 0.1);
  z-index: 9999;
  display: flex;
  flex-direction: column;
  transition: transform 0.3s ease-in-out;
  animation: slideIn 0.3s forwards;
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
  }
  to {
    transform: translateX(0%);
  }
}

.cart-header {
  display: flex;
  justify-content: space-between;
  padding: 20px;
  font-size: 18px;
  font-weight: bold;
  border-bottom: 1px solid #ccc;
}

.close-btn {
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
}

.cart-body {
  flex-grow: 1;
  overflow-y: auto;
  padding: 10px 20px;
}

.cart-item {
  display: flex;
  gap: 10px;
  margin-bottom: 15px;
}

.item-image {
  width: 60px;
  height: 60px;
  object-fit: contain;
  border: 1px solid #eee;
}

.item-info {
  flex-grow: 1;
}

.item-title {
  font-size: 14px;
  font-weight: 600;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 5px;
}

.quantity-controls button {
  padding: 4px 8px;
  border: none;
  background: #eee;
  cursor: pointer;
}

.cart-footer {
  padding: 20px;
  border-top: 1px solid #ccc;
}

.total {
  font-weight: bold;
  margin-bottom: 10px;
}

.checkout-btn {
  width: 100%;
  padding: 10px;
  background: #007BFF;
  border: none;
  color: white;
  font-weight: bold;
  border-radius: 5px;
  cursor: pointer;
}
</style>
