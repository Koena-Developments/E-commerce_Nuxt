import { ref, computed, onMounted } from 'vue'
import { defineStore } from 'pinia'

export const useMyProductStoreStore = defineStore('useProductStore', () => {
  const fakestore= ref([])
  const selectedCategory = ref('All')
  const searchTerm = ref('')
  const product = ref([])
  const filterVisible = ref(false)
  const categories = ['All', 'Clothing', 'Electronics']
  const cart = ref()
  // const cart= ref([])
  // cartVisible = ref(false)

  const fetchProducts = async () => {
    const { data: products, error } = await useFetch('https://fakestoreapi.com/products')
    if (!error.value && products.value) {
      fakestore.value = products.value.map(p => ({ ...p, quantity: 1 }))
    }
  }
  
const selectCategory = (category) => {
  selectedCategory.value = category
  filterVisible.value = false 
  // return category
}


  return {
    fakestore,
    selectedCategory,
    searchTerm,
    fetchProducts,
    product,
    selectCategory,
    categories,
    filterVisible
  }
})


