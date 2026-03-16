/**
 * Spike test FilmApi – pic de charge soudain.
 * (1) Lancer une seed (50k ou 500k) depuis le dashboard Aspire.
 * (2) Exécuter : k6 run -e BASE_URL=http://localhost:PORT -e TOTAL_ITEMS=50000 scripts/spike-test/spike-test.js
 *    ou TOTAL_ITEMS=500000.
 * Métriques → InfluxDB → Grafana.
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { getRandomFilmsPageUrl } from './lib/config.js';

export const options = {
  stages: [
    { duration: '15s', target: 30 },
    { duration: '10s', target: 30 },
    { duration: '30s', target: 150 },
    { duration: '30s', target: 150 },
    { duration: '15s', target: 0 },
  ],
  thresholds: {
    http_req_failed: ['rate<0.1'],
    http_req_duration: ['p(95)<5000'],
  },
};

export default function () {
  const url = getRandomFilmsPageUrl();
  const res = http.get(url);
  check(res, { 'status is 200': (r) => r.status === 200 });
  sleep(0.5);
}
