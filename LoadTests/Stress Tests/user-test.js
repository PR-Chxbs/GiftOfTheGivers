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
  // ----- 1️⃣ Login -----
  let loginRes = http.post(`${BASE_URL}/Auth/Login`, USER_CREDENTIALS);

  check(loginRes, {
    'login success or redirect': (r) => r.status === 200 || r.status === 302,
  });

  // Save cookies for authenticated requests
  let jar = http.cookieJar();
  let cookies = loginRes.cookies;

  // ----- Fetch events -----
  let eventsRes = http.get(`${BASE_URL}/User/Events`);
  check(eventsRes, {
    'fetched events successfully': (r) => r.status === 200,
  });

  // ----- Submit a donation -----
  const donationPayload = {
    DonorName: `PerformanceUser${__VU}-${__ITER}`,
    Amount: Math.floor(Math.random() * 100) + 50,
    ItemDescription: 'Test Donation',
    DateReceived: new Date().toISOString().split('T')[0],
    EventID: 2,
  };

  let donationRes = http.post(`${BASE_URL}/User/CreateDonation`, donationPayload);
  check(donationRes, {
    'donation created or redirected': (r) => r.status === 200 || r.status === 302,
  });

  // ----- Submit a resource -----
  const resourcePayload = {
    Name: `Resource-${__VU}-${__ITER}`,
    Quantity: 10,
    Unit: 'Boxes',
    EventID: 2,
  };

  let resourceRes = http.post(`${BASE_URL}/User/CreateResource`, resourcePayload);
  check(resourceRes, {
    'resource created or redirected': (r) => r.status === 200 || r.status === 302,
  });

  // ----- Log out -----
  let logoutRes = http.post(`${BASE_URL}/Auth/Logout`);
  check(logoutRes, {
    'logout success': (r) => r.status === 200 || r.status === 302,
  });

  sleep(1);
}
