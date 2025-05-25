<template>
  <div class="product-card">
    <nuxt-link :to="`/products/${product.id}`"><img :src="product.image" :alt="product.title" /></nuxt-link>
    <h3>{{ product.title }}</h3>
    <p>R{{ product.price }}</p>
    <button @click="$emit('add-to-cart', product)">Add to Cart</button>
  </div>
</template>

<script setup>
defineProps(['product'])
</script>

<style scoped>
.product-card {
  background: white;
  padding: 16px;
  border-radius: 12px;
  box-shadow: 0 2px 6px rgba(0,0,0,0.08);
  text-align: center;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  transition: transform 0.3s ease;
}

.product-card:hover {
  transform: scale(1.03);
}

.product-card img {
  max-width: 100px;
  max-height: 120px;
  object-fit: contain;
  margin: 0 auto 10px;
}

.product-card h3 {
  font-size: 0.9rem;
  margin: 10px 0;
  height: 40px;
  overflow: hidden;
}

.product-card p {
  font-weight: bold;
  color: #333;
}

.product-card button {
  margin-top: auto;
  background-color: #ec008c;
  border: none;
  color: white;
  padding: 6px 10px;
  border-radius: 20px;
  cursor: pointer;
  font-size: 0.85rem;
}
.product-card button:hover {
  background-color: #c70074;
}
</style>
