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
  padding: 16px;
  border: 1px solid #ddd;
  transition: 0.3s ease;
  overflow: hidden;
}

.product-image {
  width: 100%;
  height: 200px;
  object-fit: contain;
}

.product-info {
  text-align: center;
}

.hover-slider {
  position: absolute;
  bottom: 16px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 10px;
  opacity: 0;
  transition: 0.3s ease;
}

.product-card:hover .hover-slider {
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
  box-shadow: 0 0 10px #00f;
}
</style>
