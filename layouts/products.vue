<template>
    <div>
        <header>
            <NuxtLink to="/products">PLUTO-WEAR</NuxtLink>  
        </header>
    </div>

    <!-- where the platform that is inheriting is gonna display -->
    <div>
        <slot />
    </div>

    <footer class="footer-class">
     <nav class="container">     
     <ul class="links">
         <li><NuxtLink to="/"> Home </NuxtLink></li>
         <li><NuxtLink to="/about"> about </NuxtLink></li>
         <li><NuxtLink to="/products"> products </NuxtLink></li>
     </ul>
    
     </nav>
    </footer>
</template>

<script setup>

</script>

<style scoped>
.router-link-exact-active
{
    color: orange;
}

.footer-class
{
    background-color:white;
    padding: 1rem 0;
}

.container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 1rem;
}

.logo {
    font-size: 1.5rem;
    font-weight: bold;
    color: black;
    text-decoration: none;
}

.links {
    list-style: none;
    display: flex;
    gap: 1.5rem;
}

.links li {
    margin: 0;
}

.links a {
    text-decoration: none;
    font-size: 1rem;
    transition: color 0.3s;
    font-size:20px;

}

.links a:hover {
    color: #4daf6b;
}

.router-link-exact-active {
    color: #4daf6b;
}
</style>

