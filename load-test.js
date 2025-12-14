import http from 'k6/http';
import { check } from 'k6';

export const options = {
    scenarios: {
        cpu_and_memory_pressure: {
            executor: 'constant-arrival-rate',
            rate: 200,              // 200 req/s
            timeUnit: '1s',
            duration: '5m',
            preAllocatedVUs: 50,
            maxVUs: 300,
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.05'],
        http_req_duration: ['p(95)<2000'],
    },
};

const url = 'http://4.229.164.229/Users';

const params = {
    headers: {
        'accept': 'text/plain',
        'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJGUkFOSy5WSUVJUkEiLCJ1c2VyX2lkIjoiRlJBTksuVklFSVJBIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IkZSQU5LIFZJRUlSQSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzY1NjgyNzE3LCJpc3MiOiJGQ0cuVXNlcnMifQ.GoZ7hLuw86_STebQkMqbn30Nh8B3jAn2Mck5UW75k5U',
    },
};

export default function () {
    const res = http.get(url, params);

    // força uso de memória ao manter o body
    const body = res.body;
    const parsed = body ? body.length : 0;

    check(res, {
        'status 200': r => r.status === 200,
    });

    // pequeno loop pra gastar CPU no pod
    let cpuBurn = 0;
    for (let i = 0; i < 50000; i++) {
        cpuBurn += Math.sqrt(i * parsed);
    }
}