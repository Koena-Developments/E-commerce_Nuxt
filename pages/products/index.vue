<template>
  <div class="landing">
    <h1>Products World</h1>
    <ProductSlider :products="products" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import ProductSlider from '~/components/ProductSlider.vue'

const products = ref([])

const fetchProducts = async () => {
  const { data, error } = await useFetch('https://fakestoreapi.com/products')
  if (!error.value) {
    products.value = data.value || []
  }
}

onMounted(fetchProducts)
</script>

<style scoped>
.landing {
  padding: 40px 20px;
  text-align: center;
}
.landing h1 {
  font-size: 2.5rem;
  margin-bottom: 24px;
}
</style>
