<template>
  <div class="product-card">
    <img :src="product.image" :alt="product.title" class="product-image" />

    <div class="product-info">
      <h3 class="product-title">{{ product.title }}</h3>
      <p class="product-price">R{{ product.price }}</p>
    </div>

    <div class="hover-slider">
      <button class="action-button" @click="$emit('add-to-cart',product)">
        <i class="fas fa-cart-plus"></i>
      </button>

      <button class="action-button" @click="$emit('remove-product', product)">
        <i class="fas fa-trash"></i>
      </button>
     
      <NuxtLink :to="`/products/${product.id}`">
      <button class="action-button" @click="$emit('view-product', product)">
        <i class="fas fa-eye"></i>
      </button>
      </NuxtLink>
    
    </div>
  </div>
</template>

<script setup>
defineProps({
  product: Object
})

defineEmits(['add-to-cart', 'remove-product', 'view-product'])
</script>

<style scoped>
.product-card {
  position: relative;
  padding: 0 10px 0;
  border: 1px solid #ddd;
  transition: 0.3s ease;
  overflow: hidden;
  margin-top: 20px;
  width: 80%;
  display: flex;
  flex-direction: column;
  align-items: center;
  box-shadow:0 15px 5px rgb(219, 208, 208);
}

.product-image {
  width: 60%;
  height: 200px;
  object-fit: contain;
}

.product-info {
  text-align: center;
}

.hover-slider {
  position: absolute;
  left: 50%;
  bottom: 0;
  transform: translate(-50%, 30px);
  display: flex;
  justify-content: center;
  gap: 10px;
  opacity: 0.2;
  background: rgb(214, 210, 210);
  width: 100%;
  transition: opacity 0.3s ease, transform 0.3s ease;
  border-radius: 15px 15px 0 2px;
}

.product-card:hover .hover-slider {
  opacity: 1;
  transform: translate(-50%, 0);
}

@keyframes slideup {
  from {
    transform: translate(-50%, 30px);
    opacity: 0;
  }
  to {
    transform: translate(-50%, 0);
    opacity: 1;
  }
}
.product-card:hover  {
  opacity: 1;
}

.action-button {
  border: none;
  border-radius: 50%;
  padding: 10px;
  background-color: #f0f0f0;
  cursor: pointer;
  transition: box-shadow 0.3s ease;
}

.action-button:hover {
  box-shadow: 0 0 10px rgb(74, 92, 24);
}
</style>
