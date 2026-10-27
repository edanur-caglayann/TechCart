import { spawn } from "node:child_process";
import { createRequire } from "node:module";
import { fileURLToPath } from "node:url";

const apiUrls = {
  local: "http://localhost:5223",
  docker: "http://localhost:8080",
  clean: "http://localhost:8081",
};

const [profile, ...nextArguments] = process.argv.slice(2);

if (!Object.hasOwn(apiUrls, profile)) {
  console.error("Ortam secimi local, docker veya clean olmalidir.");
  process.exit(1);
}

const require = createRequire(import.meta.url);
const nextCli = require.resolve("next/dist/bin/next");
const projectDirectory = fileURLToPath(new URL("../", import.meta.url));

console.info(`TechCart frontend: ${profile} -> ${apiUrls[profile]}`);

// process.env, Next.js'in .env.local dosyasindan once gelir.
const child = spawn(
  process.execPath,
  [nextCli, "dev", "--port", "3000", ...nextArguments],
  {
    cwd: projectDirectory,
    env: { ...process.env, NEXT_PUBLIC_API_URL: apiUrls[profile] },
    stdio: "inherit",
  },
);

for (const signal of ["SIGINT", "SIGTERM"]) {
  process.on(signal, () => child.kill(signal));
}

child.on("error", (error) => {
  console.error("Frontend baslatilamadi:", error.message);
  process.exit(1);
});

child.on("exit", (code, signal) => {
  process.exit(code ?? (signal === "SIGINT" ? 130 : 1));
});
