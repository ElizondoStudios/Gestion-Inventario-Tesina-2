import { createRequire } from "node:module";

const require = createRequire(import.meta.url);
const fg = require("fast-glob");
const fs = require("node:fs");
const path = require("node:path");
const escomplex = require("typhonjs-escomplex");

// 1) Ajusta estos patrones según tu proyecto:
const include = [
  "src/**/*.{js,jsx,ts,tsx}",
  "app/**/*.{js,jsx,ts,tsx}",
  "services/**/*.{js,jsx,ts,tsx}",
  "DTOs/**/*.{js,jsx,ts,tsx}"
];

const ignore = [
  "**/*.d.ts",
  "**/node_modules/**",
  "**/dist/**",
  "**/build/**",
  "**/.next/**",
  "**/.react-router/**"
];

const files = fg.sync(include, { ignore });

// 2) Fuentes para escomplex
const sources = files.map((f) => ({
  srcPath: f.replace(/\\/g, "/"),
  code: fs.readFileSync(f, "utf8")
}));

// 3) Parser Babel compatible TS/JSX
const parserOptions = {
  sourceType: "module",
  plugins: [
    "jsx",
    "typescript",
    "classProperties",
    "objectRestSpread",
    "dynamicImport",
    "decorators-legacy"
  ]
};

// 4) Análisis
const result = escomplex.analyzeProject(sources, { ignoreErrors: true }, parserOptions);
const modules = result.modules || [];

// 5) Métricas por archivo
const rows = modules.map((m) => ({
  Jerarquia: path.dirname(m.srcPath).replace(/\\/g, "/"),
  Archivo: m.srcPath,
  IndiceMantenibilidad: Number((m.maintainability ?? 0).toFixed(3)),
  ComplejidadCiclomatica: m.aggregate?.cyclomatic ?? 0,
  LineasCodigoFuente: m.aggregate?.sloc?.physical ?? 0,
  LineasCodigoEjecutable: m.aggregate?.sloc?.logical ?? 0
}));

rows.sort((a, b) => a.Jerarquia.localeCompare(b.Jerarquia) || a.Archivo.localeCompare(b.Archivo));

// 6) CSV detalle
const headers = [
  "Jerarquia",
  "Archivo",
  "IndiceMantenibilidad",
  "ComplejidadCiclomatica",
  "LineasCodigoFuente",
  "LineasCodigoEjecutable"
];
const esc = (v) => `"${String(v).replace(/"/g, '""')}"`;
const csv = [headers.join(","), ...rows.map((r) => headers.map((h) => esc(r[h])).join(","))].join("\n");
fs.writeFileSync("escomplex-metrics-report.csv", "\uFEFF" + csv, "utf8");

// 7) Resumen proyecto (JSON)
const summary = {
  archivosAnalizados: files.length,
  indiceMantenibilidadProyecto: Number((result.moduleAverage?.maintainability ?? 0).toFixed(3)),
  complejidadCiclomaticaTotal: rows.reduce((a, b) => a + b.ComplejidadCiclomatica, 0),
  lineasCodigoFuenteTotal: rows.reduce((a, b) => a + b.LineasCodigoFuente, 0),
  lineasCodigoEjecutableTotal: rows.reduce((a, b) => a + b.LineasCodigoEjecutable, 0)
};
fs.writeFileSync("escomplex-summary.json", JSON.stringify(summary, null, 2), "utf8");

console.log("OK: escomplex-metrics-report.csv + escomplex-summary.json");