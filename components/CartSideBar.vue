<template>
  <aside class="cart-sidebar" :class="{ visible }">
    <header class="cart-header">
      <h3>Your Cart</h3>
      <button class="close-btn" @click="$emit('close-cart')">✖</button>
    </header>

    <div class="cart-body" v-if="cart.length">
      <div v-for="item in cart" :key="item.id" class="cart-item">
        <img :src="item.image" alt="" class="item-image"/>
        <div class="item-info">
          <p class="item-title">{{ item.title }}</p>
          <div class="quantity-controls">
            <button @click="$emit('decrease', item)">−</button>
            <span>{{ item.quantity }}</span>
            <button @click="$emit('increase', item)">+</button>
          </div>
        </div>
      </div>
    </div>
    <p v-else class="empty-msg">Your cart is empty.</p>

    <footer class="cart-footer">
      <p class="total">Total: R{{ total }}</p>
      <button class="checkout-btn" @click="$emit('checkingout')">Checkout</button>
    </footer>
  </aside>
</template>

<script setup>
defineProps({
  cart:   { type: Array,   required: true },
  visible:{ type: Boolean, required: true },
  total:  { type: [Number,String], required: true }
})
defineEmits(['close-cart','increase','decrease','checkingout'])
</script>

<style scoped>
.cart-sidebar {
  position: fixed;
  top: 0; 
  right: 0;
  width: 350px;
  height: 100%;
  background: #fff;
  box-shadow: -4px 0 8px rgba(0,0,0,0.1);
  transform: translateX(100%);
  transition: transform 0.3s ease;
  z-index: 1000;
  display: flex;
  flex-direction: column;
}
.cart-sidebar.visible {
  transform: translateX(0);
}
.cart-header, .cart-footer {
  padding: 16px;
  border-bottom: 1px solid #ddd;
}
.cart-header {
  display: flex; justify-content: space-between; align-items: center;
}
.close-btn {
  background: none; border: none; font-size: 1.2rem; cursor: pointer;
}
.cart-body {
  flex-grow: 1;
  overflow-y: auto;
  padding: 8px 16px;
}
.empty-msg {
  padding: 16px;
  text-align: center;
  color: #666;
}
.cart-item {
  display: flex;
  margin-bottom: 12px;
}
.item-image {
  width: 60px; height: 60px;
  object-fit: contain;
  border: 1px solid #eee;
  margin-right: 12px;
}
.item-info {
  flex: 1;
}
.item-title {
  font-size: 0.95rem;
  margin-bottom: 8px;
}
.quantity-controls {
  display: flex;
  align-items: center;
  gap: 8px;
}
.quantity-controls button {
  width: 24px; height: 24px;
  border: none; background: #f0f0f0;
  border-radius: 4px;
  cursor: pointer;
}
.cart-footer {
  border-top: 1px solid #ddd;
}
.total {
  font-weight: bold;
  margin-bottom: 12px;
}
.checkout-btn {
  width: 100%;
  padding: 10px;
  background: #ec008c;
  border: none;
  color: #fff;
  border-radius: 4px;
  cursor: pointer;
}
.checkout-btn:hover 
{
   background: #a50364; 
   }
</style>
