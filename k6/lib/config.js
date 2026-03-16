/**
 * Configuration partagée pour les scripts k6 (FilmApi).
 * BASE_URL : URL de l'API. Sous Aspire (k6 en --network host), BASE_URL_HOST + BASE_URL_PORT
 *   sont injectés (port résolu par Aspire). Sinon BASE_URL ou services__film-api__http__0. En CLI : -e BASE_URL=...
 * TOTAL_ITEMS : nombre d'éléments en base (50000 ou 500000 après seed).
 * PAGE_SIZE : taille de page (défaut 100), doit correspondre à l'API /films.
 */
export const BASE_URL = __ENV.services__filmApp__https__0;

export const TOTAL_ITEMS = __ENV.TOTAL_ITEMS ? parseInt(__ENV.TOTAL_ITEMS, 10) : null;

export const PAGE_SIZE = __ENV.PAGE_SIZE ? parseInt(__ENV.PAGE_SIZE, 10) : 100;

export function getApiUrl(path = '') {
  const base = BASE_URL.replace(/\/$/, '');
  const p = (path || '').replace(/^\//, '');
  return p ? `${base}/${p}` : base;
}

export function requireTotalItems() {
  if (TOTAL_ITEMS == null || TOTAL_ITEMS <= 0) {
    throw new Error(
      'TOTAL_ITEMS est obligatoire. Lancer d\'abord une seed (50k ou 500k), puis ex. : k6 run -e TOTAL_ITEMS=50000 -e BASE_URL=http://localhost:XXXX scripts/load-test/load-test.js'
    );
  }
}

export function getMaxPage() {
  requireTotalItems();
  return Math.max(1, Math.ceil(TOTAL_ITEMS / PAGE_SIZE));
}

export function getRandomFilmsPageUrl() {
  const maxPage = getMaxPage();
  const page = maxPage <= 1 ? 1 : Math.floor(Math.random() * maxPage) + 1;
  return getApiUrl(`/films?page=${page}&pageSize=${PAGE_SIZE}`);
}
