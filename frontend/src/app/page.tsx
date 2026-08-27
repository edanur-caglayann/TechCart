import { Suspense } from "react";
import CategoryMenu from "../components/CategoryMenu/CategoryMenu";
import Filters from "../components/Filters/Filters";
import Header from "../components/Header/Header";
import Hero from "../components/Hero/Hero";
import styles from "./page.module.css";
import ProductList from "../components/ProductList/ProductList";
import Footer from "../components/Footer/Footer";

export default function HomePage() {
  return (
    <>
      <Header />

      <main>
        <Hero />

        <CategoryMenu />

        <section
          className={styles.productSection}
          id="products"
        >
          <Suspense fallback={<p>Filtreler yükleniyor...</p>}>
            <Filters />
          </Suspense>

          <div className={styles.productArea}>
            <Suspense fallback={<p>Ürünler yükleniyor...</p>}>
              <ProductList />
            </Suspense>
          </div>
        </section>
      </main>

      <Footer />
    </>
  );
}