import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 10 },   // Warm-up: slowly ramp to 10 users
    { duration: '1m', target: 50 },    // Ramp up to 50 users
    { duration: '2m', target: 100 },   // Moderate sustained load
    { duration: '1m', target: 200 },   // Heavy load
    { duration: '30s', target: 400 },  // Spike to extreme load (stress point)
    { duration: '1m', target: 400 },   // Hold at max load to observe failures
    { duration: '30s', target: 0 },    // Ramp down gracefully
  ],

  thresholds: {
    http_req_duration: ['p(95)<1000'], // 95% of requests should complete < 1s
    http_req_failed: ['rate<0.05'],    // Less than 5% of requests should fail
  },

  noConnectionReuse: false, // Reuse connections for realistic behavior
  discardResponseBodies: true, // Save memory during high load
};

const BASE_URL = 'http://localhost:5292';
const USER_CREDENTIALS = {
  Email: 'user@gift.org',
  Password: 'Test123!',
  RememberMe: false,
};

export default function () {
  // 1. Test Login
  const loginRes = http.post(`${BASE_URL}/Auth/Login`, USER_CREDENTIALS);

  check(loginRes, {
    'login status is 200 or redirect': (r) => r.status === 200 || r.status === 302,
    'login page contains redirect': (r) => r.body.includes('User') || r.body.includes('Redirect'),
  });

  // 2. Wait a bit (simulate browsing)
  sleep(1);

  // 4. Optional logout
  const logoutRes = http.post(`${BASE_URL}/Auth/Logout`);
  check(logoutRes, { 'logout status is 200 or redirect': (r) => r.status === 200 || r.status === 302 });

  sleep(1);
}
