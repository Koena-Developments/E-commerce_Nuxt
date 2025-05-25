<template>
  <div class="page-wrapper" :class="{ 'filter-open': filterVisible, 'visible':visible, 'cart-open': cartVisible }">

    <FilterSidebar
      :categories="categories"
      :selected="selectedCategory"
      :visible="filterVisible"
      @select="selectCategory"
      @toggle="filterVisible = !filterVisible"
    />


    
    <!-- Main Product Grid -->
    
    <main class="content-area">
      <div class="toolbar">
        <button class="btn" @click="filterVisible = !filterVisible">
          {{ filterVisible ? 'Hide Filters' : 'Show Filters' }}
        </button>
      </div>
        <!-- <main class="content-area">
      <HeroSection v-model="searchTerm" />
    </main> -->

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
    </main>

    <!-- Cart Sidebar -->
    <CartSidebar
      :cart="cart"
      :visible="cartVisible"
      :total="cartTotal"
      @close-cart="cartVisible = false"
      @increase="increaseQuantity"
      @decrease="decreaseQuantity"
      @checkingout="checkout"
    />


    <footer />
        </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import ProductCard from '~/components/ProductCard.vue'
import FilterSidebar from '~/components/FilterSidebar.vue'
import CartSidebar from '~/components/CartSideBar.vue'
import footer  from '~/components/footer.vue'
import HeroSection from '~/components/hero-section.vue'


const fakestore = ref([])
const cart = ref([])
const filterVisible = ref(false)
const cartVisible = ref(false)
const searchTerm = ref('')
const categories = ['All', 'Clothing', 'Electronics']
const selectedCategory = ref('All')

const fetchProducts = async () => {
  const { data: products, error } = await useFetch('https://fakestoreapi.com/products')
  if (!error.value && products.value) {
    fakestore.value = products.value.map(p => ({ ...p, quantity: 1 }))
  }
}
onMounted(fetchProducts)

const filteredProducts = computed(() => {
  let prods = fakestore.value
  if (selectedCategory.value !== 'All') {
    prods = prods.filter(p => p.category.toLowerCase() === selectedCategory.value.toLowerCase())
  }
  
  if(searchTerm.value){
    const lowerCaseSearchTerm = searchTerm.value.toLocaleLowerCase();
    prods = prods.filter(p=> p.title.toLowerCase().include*lowerCaseSearchTerm) || p.description.toLowerCase().includes(lowerCaseSearchTerm);
  }
  
  return prods

})


const selectCategory = (cat) => {
  selectedCategory.value = cat
  filterVisible.value = false
}

const addToCart = (product) => {
  const existing = cart.value.find(p => p.id === product.id)
  if (existing) {
    existing.quantity++
  } else {
    cart.value.push({ ...product })
  }
  cartVisible.value = true
}
const removeProduct = (product) => {
  cart.value = cart.value.filter(p => p.id !== product.id)
  if (!cart.value.length) cartVisible.value = false
}
const increaseQuantity = (product) => {
   product.quantity++ 
  }
const decreaseQuantity = (product) => {
  if (product.quantity > 1) product.quantity--
  else removeProduct(product)
}

const cartTotal = computed(() =>
  cart.value.reduce((sum, i) => sum + i.price * i.quantity, 0).toFixed(2)
)

const checkout = () => {
  if (!cart.value.length) return alert('Cart is empty')
  alert(`Checking out ${cart.value.length} items – total $${cartTotal.value}`)
  cart.value = []
  cartVisible.value = false
}
</script>

<style scoped>

.page-wrapper {
  display: flex;
  position: relative;
  transition: transform 0.3s ease;
  overflow: hidden;
}
/* .page-wrapper.filter-open { transform: translateX(200px); } */
/* .page-wrapper.cart-open   { transform: translateX(-60px); } */

.content-area {
  flex-grow: 1;
  padding: 20px;
  display: flex;
  flex-direction: column;
}
.toolbar {
  margin-bottom: 12px;
}
.btn {
  padding: 8px 12px;
  border: none;
  background: #007bff;
  color: white;
  border-radius: 4px;
  cursor: pointer;
}
.btn:hover { background: #0056b3; }

.listProduct {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 16px;
}
</style>
