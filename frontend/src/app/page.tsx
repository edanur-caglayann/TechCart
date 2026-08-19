import CategoryMenu from "../components/CategoryMenu/CategoryMenu";
import Filters from "../components/Filters/Filters";
import Header from  "../components/Header/Header";
import Hero from "../components/Hero/Hero";
import styles from "./page.module.css";
import ProductList from "../components/ProductList/ProductList";


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
          <Filters />

          <div className={styles.productArea}>
            <ProductList />
          </div>
          
        </section>
      </main>
    </>
  );
}