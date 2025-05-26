import { ref, computed, onMounted } from 'vue'
import { defineStore } from 'pinia'

export const useMyProductStoreStore = defineStore('useProductStore', () => {
  const fakestore= ref([])
  const selectedCategory = ref('All')
  const searchTerm = ref('')
  const product = ref([])
  // const cart= ref([])
  // cartVisible = ref(false)

  const fetchProducts = async () => {
    const { data: products, error } = await useFetch('https://fakestoreapi.com/products')
    if (!error.value && products.value) {
      fakestore.value = products.value.map(p => ({ ...p, quantity: 1 }))
    }
  }
  
  // const filteredProducts = computed(() => {
  //   let prods = fakestore.value
  //   if (selectedCategory.value !== 'All') {
  //     prods = prods.filter(p => p.category.toLowerCase() === selectedCategory.value.toLowerCase())
  //   }

  //   if (searchTerm.value) {
  //     const lowerCaseSearchTerm = searchTerm.value.toLowerCase();
  //     prods = prods.filter(
  //       p =>
  //         p.title.toLowerCase().includes(lowerCaseSearchTerm) ||
  //         p.description.toLowerCase().includes(lowerCaseSearchTerm)
  //     );
  //   }
  //   return prods
  // })

  return {
    fakestore,
    selectedCategory,
    searchTerm,
    fetchProducts,
    product

  }
})


