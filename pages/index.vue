<template>
  <div class="listProduct">
    <ProductCard
      v-for="product in filteredProducts"
      :key="product.id"
      :product="product"
      @add-to-cart="addToCart"
      @remove-product="removeProduct"
      @view-product="viewProduct"
    />
  </div>

  <CartSidebar
    :cart="cart"
    :visible="cartVisible"
    :total="cartTotal"
    @close-cart="cartVisible = false"
    @increase="increaseQuantity"
    @decrease="decreaseQuantity"
  />

</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import ProductCard from '~/components/ProductCard.vue'
import CartSidebar from '~/components/CartSidebar.vue'

const fakestore = ref([])
const cart = ref([])
const searchTerm = ref('')
const cartVisible = ref(false)

const filteredProducts = computed(() => {
  const term = searchTerm.value.toLowerCase()
  return fakestore.value.filter(product =>
    product.title.toLowerCase().includes(term)
  )
})

const fetchProducts = async () => {
   const { data: products, error } = await useFetch('https://fakestoreapi.com/products')
    console.log('products', products)
  if (error.value) {
    console.error('Fetch error:', error.value)
    fakestore.value = []
  } else {
    fakestore.value = products.value || []
  }
}

const addToCart = (product) => {
  const existing = cart.value.find(p => p.id === product.id)
  if (existing) {
    existing.quantity++
  } else {
    cart.value.push({ ...product, quantity: 1 })
  }
  cartVisible.value = true
}


const removeProduct = (product) => {
  const index = cart.value.indexOf(product) 
  if (index > -1) {
    cart.value.splice(index, 1)
  }
   if (cart.value.length === 0) 
    cartVisible.value = false
}


const increaseQuantity = (product) => {
  product.quantity++
}

const decreaseQuantity = (product) => {
  if (product.quantity > 1) {
    product.quantity--
  } else {
    removeProduct(product)
  }
}

const cartTotal = computed(() =>
  cart.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
)

const cartTotalFloat = computed(() => parseFloat(cartTotal.value))

onMounted (async () => {
await fetchProducts()
})



</script>

<style scoped>
.listProduct {
  display: grid;
  grid-template-columns: repeat(4, 1fr); 
  gap: 15px; 
}

@media (max-width: 800px) {
  .listProduct {
    grid-template-columns: repeat(3, 1fr); 
  }
}
</style>
