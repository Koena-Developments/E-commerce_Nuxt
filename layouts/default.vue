<template>
    <div>
        <header class="header-class">
            <nav class="container">
                <NuxtLink to="/" class="logo">Pluto_Wear</NuxtLink>
                
                <ul class="links">
                    <li><NuxtLink to="/"> Home </NuxtLink></li>
                    <li><NuxtLink to="/about"> About </NuxtLink></li>
                    <li><NuxtLink to="/products"> Products </NuxtLink></li>
                </ul>
            </nav>
        </header>
    </div>

    <div class="content">
        <slot />
    </div>
</template>

<script setup>
</script>

<style scoped>
.content
{
    margin-left: 1rem;
    margin-right: 1rem;
}
.header-class {
    background-color: #333;
    padding: 1rem 0;
    color: white;
    display: flex;
    justify-content: center;
    align-items: center;
    height: 1vh;
}

.container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    max-width: 1200px;
    width: 100%;
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
    color: black;
    text-decoration: none;
    font-size: 1.3rem;
    transition: color 0.3s;
}

.links a:hover {
    color: #4daf6b;
}

.router-link-exact-active {
    color: #4daf6b;
}
</style>
