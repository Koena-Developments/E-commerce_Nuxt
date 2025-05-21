<template>
  <div class="listProduct">
    <RevisedproductCard
      v-for="product in filteredProducts"
      :key="product.id"
      :product="product"
      @add-to-cart="addToCart"
      @remove-product="removeProduct"
      @view-product="viewProduct"  
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import RevisedproductCard from '~/components/revisedproductCard.vue'

const fakestore = ref([])
const cart = ref([])
const searchTerm = ref('')

const filteredProducts = computed(() => {
  const term = searchTerm.value.toLowerCase()
  return fakestore.value.filter(product =>
    product.title.toLowerCase().includes(term)
  )
})


const fetchProducts = async () => {
  const { data: products, error } = await useFetch('https://fakestoreapi.com/products')

  if (error.value) {
    console.error('Fetch error:', error.value)
    fakestore.value = []
  } else {
    fakestore.value = products.value || []
  }
}

  const addToCart = (product) => {
  if (!cart.value.find(p => p.id === product.id)) {
    cart.value.push(product)
  }
}


const removeProduct = (product) => {
  const index = cart.value.indexOf(product)
  if (index > -1) {
    cart.value.splice(index, 1)
  }
}

onMounted(fetchProducts)
</script>

<style scoped>
.listProduct {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 10px;
}
</style>
