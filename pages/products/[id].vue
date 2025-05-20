<template>
    <div>
        <h2>product page this is product ID: {{ id }}</h2>
        <p>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Commodi aliquid laudantium saepe similique aspernatur voluptate omnis, aut debitis nulla animi dolores voluptas est repudiandae sed minus et. At, culpa alias.</p>
    </div>
</template>

<script setup>
const {id} = useRoute().params
definePageMeta({
    layout:'products'
})


</script>

<style scoped>

</style>