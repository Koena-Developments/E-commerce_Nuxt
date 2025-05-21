<template>
  <div>
    <ProductDetail :id="productId" />
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router';
import ProductDetail from '@/components/viewProduct.vue'; 

const route = useRoute();
const productId = route.params.id;
definePageMeta({
  layout: 'products'
});
</script>

<style scoped>
</style>