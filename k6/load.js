/**
 * Load test FilmApi – charge type production.
 * (1) Lancer une seed (50k ou 500k) depuis le dashboard Aspire.
 * (2) Exécuter : k6 run -e BASE_URL=http://localhost:PORT -e TOTAL_ITEMS=50000 scripts/load-test/load-test.js
 *    ou TOTAL_ITEMS=500000 pour la grosse seed.
 * Métriques → InfluxDB → Grafana (dashboard k6 Load Testing).
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { getRandomFilmsPageUrl } from './lib/config.js';

export const options = {
  stages: [
    { duration: '30s', target: 50 },
    { duration: '1m', target: 50 },
    { duration: '30s', target: 0 },
  ],
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<2000', 'p(99)<4000'],
  },
};

export default function () {
  const url = getRandomFilmsPageUrl();
  const res = http.get(url);
  check(res, { 'status is 200': (r) => r.status === 200 });
  sleep(1);
}
