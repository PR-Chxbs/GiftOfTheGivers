import http from 'k6/http';
import { check, sleep } from 'k6';

// const virtualUsers = 5; // Test One & Two
// const virtualUsers = 40; // Test Three
const virtualUsers = 100; // Test Four

export const options = {
  vus: virtualUsers,
  duration: '15s', // test duration
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
