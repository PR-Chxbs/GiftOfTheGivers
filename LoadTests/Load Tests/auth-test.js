import http from 'k6/http';
import { check, sleep } from 'k6';

const virtualUsers = 5; // Test One
// const virtualUsers = 20; // Test Two

export const options = {
  vus: virtualUsers,
  duration: '10s', // total duration
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
