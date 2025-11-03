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
const ADMIN_CREDENTIALS = {
  Email: 'admin@gift.org',
  Password: 'Test123!',
  RememberMe: false,
};

export default function () {
  // ----- Login as Admin -----
  let loginRes = http.post(`${BASE_URL}/Auth/Login`, ADMIN_CREDENTIALS);

  check(loginRes, {
    'admin login success or redirect': (r) => r.status === 200 || r.status === 302,
  });

  // ----- Access Admin Dashboard -----
  let indexRes = http.get(`${BASE_URL}/Admin/Index`);
  check(indexRes, {
    'admin dashboard loads': (r) => r.status === 200,
  });

  // ----- List Users -----
  let usersRes = http.get(`${BASE_URL}/Admin/Users`);
  check(usersRes, { 'users page loads': (r) => r.status === 200 });

  // ----- Create Event -----
  const eventPayload = {
    Name: `Relief Event ${__VU}-${__ITER}`,
    Location: 'Test City',
    StartDate: new Date().toISOString().split('T')[0],
    EndDate: new Date().toISOString().split('T')[0],
    Description: 'Load test event',
  };

  let createEventRes = http.post(`${BASE_URL}/Admin/CreateEvent`, eventPayload);
  check(createEventRes, {
    'event created or redirected': (r) => r.status === 200 || r.status === 302,
  });

  // ----- List Events -----
  let eventsRes = http.get(`${BASE_URL}/Admin/Events`);
  check(eventsRes, { 'events page loads': (r) => r.status === 200 });

  // ----- Create Donation -----
  const donationPayload = {
    DonorName: `AdminDonor${__VU}-${__ITER}`,
    Amount: Math.floor(Math.random() * 200) + 100,
    ItemDescription: 'Admin test donation',
    DateReceived: new Date().toISOString().split('T')[0],
    EventID: 2,
  };

  let createDonationRes = http.post(`${BASE_URL}/Admin/CreateDonation`, donationPayload);
  check(createDonationRes, {
    'donation created or redirected': (r) => r.status === 200 || r.status === 302,
  });

  // ----- Create Resource -----
  const resourcePayload = {
    Name: `AdminRes${__VU}-${__ITER}`,
    Quantity: 15,
    Unit: 'Crates',
    EventID: 2,
  };

  let createResourceRes = http.post(`${BASE_URL}/Admin/CreateResource`, resourcePayload);
  check(createResourceRes, {
    'resource created or redirected': (r) => r.status === 200 || r.status === 302,
  });

  // ----- Create Assignment -----
  const assignmentPayload = {
    UserID: '1ec9573a-252d-40e1-b879-b359e2ac3620', // User (user@gift.org) ID
    EventID: 2,
    RoleInProject: 'Coordinator',
  };

  let createAssignRes = http.post(`${BASE_URL}/Admin/CreateAssignment`, assignmentPayload);
  check(createAssignRes, {
    'assignment created or redirected': (r) => r.status === 200 || r.status === 302,
  });

  // ----- Log out -----
  let logoutRes = http.post(`${BASE_URL}/Auth/Logout`);
  check(logoutRes, {
    'admin logout success': (r) => r.status === 200 || r.status === 302,
  });

  sleep(1);
}
