import Header from  "../components/Header/Header";
import Hero from "../components/Hero/Hero";


export default function HomePage() {
  return (
    <>
      <Header />

      <main>
        <Hero />

        <section id="products">
          <h2>Ürünleri Keşfet</h2>
        </section>
      </main>
    </>
  );
}