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
    
    <Footer />
  
  <CartSidebar
    :cart="cart"
    :visible="cartVisible"
    :total="cartTotal"
    @close-cart="cartVisible = false"
    @increase="increaseQuantity"
    @decrease="decreaseQuantity"
    @checkingout="checkout"
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

// stripe integration
const checkout = () => {
  if (!cart.value.length) {
    return alert('Cart is empty')
  }
  const paymentLinks = {
      1:  'https://buy.stripe.com/test_eVqaEZ6L6bgi5wMgOD1wY01', 
      2:  'https://buy.stripe.com/test_6oUeVfedy702aR655V1wY02',
      3:  'https://buy.stripe.com/test_6oU5kFglG3NQ2kA8i71wY03', 
      4:  'https://buy.stripe.com/test_dRm3cx3yU7023oEbuj1wY04',
      5:  'https://buy.stripe.com/test_cNi4gB6L60BEf7mgOD1wY05',
      6:  'https://buy.stripe.com/test_3cI5kF3yUace3oE55V1wY07',
      7:  'https://buy.stripe.com/test_00w9AVc5q8465wMcyn1wY06',
      8:  'https://buy.stripe.com/test_9B6aEZ1qM846aR6eGv1wY08',
      9:  'https://buy.stripe.com/test_28EbJ3d9udoqgbqaqf1wY09',
      10: 'https://buy.stripe.com/test_cNieVfb1m3NQcZe41R1wY0a',
      11: 'https://buy.stripe.com/test_fZu3cxb1m3NQ2kAeGv1wY0b',
      12: 'https://buy.stripe.com/test_6oU00lb1mgAC7EUgOD1wY0d',
      13: 'https://buy.stripe.com/test_7sY28t6L60BE6AQfKz1wY0e',
      14: 'https://buy.stripe.com/test_dRm28t9Xi2JM6AQdCr1wY0f',
      15: 'https://buy.stripe.com/test_7sY4gBd9u2JMaR669Z1wY0g',
      16: 'https://buy.stripe.com/test_eVq7sN8Teesu9N2gOD1wY0h',
      17: 'https://buy.stripe.com/test_8x2aEZ7Pa8461gw55V1wY0i',
      18: 'https://buy.stripe.com/test_4gM6oJ5H21FI1gweGv1wY0j',
      19: 'https://buy.stripe.com/test_00w3cx1qMgAC3oEdCr1wY0k',
      20: 'https://buy.stripe.com/test_fZubJ36L698acZe69Z1wY0l'
    };
  const link = paymentLinks[cart.value[0].id]
  if (link) {
    window.location.href = link
  } else {
    alert(`Checked out ${cart.value.length} items totaling R${total.value.toFixed(2)}`)
    clearCart()
  }
}









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
