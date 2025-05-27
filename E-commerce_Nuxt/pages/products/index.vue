<template>
  <div class="landing">
    <h1>Products World</h1>
    <ProductShow :products="fakestore" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import ProductShow from '~/components/ProductShowRoom.vue'
import { useMyProductStoreStore } from '~/stores/productStore'

const {fetchProducts} = useMyProductStoreStore()
const {fakestore} = storeToRefs(fetchProduct)

onMounted(() => {
 fetchProducts()
})
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
