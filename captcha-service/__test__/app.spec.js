jest.mock('svg-captcha')

const supertest = require('supertest')
const app = require('../src/app')

let server
let request

beforeAll(() => {
    server = app.listen(12001)
    request = supertest(server)
})

afterAll(() => {
    server.close()
})

describe('/api/generate', () => {
    it('可以生成验证码', async () => {
        const res = await request.post('/api/generate')
            .expect(200)
        expect(res.body).toMatchObject({
            error: false,
            data: {
                id: expect.any(String),
                captcha: '<svg/>',
            },
        })
    })
})

describe('/api/validate', () => {

    it('能够进行验证', async () => {
        const id = (await request.post('/api/generate')
            .expect(200)).body.data.id

        await request.post('/api/validate')
            .send({ id, text: 'validate-text' })
            .expect(200, {
                error: false,
                data: true,
            })
    })

    it('不论大小写', async () => {
        const id = (await request.post('/api/generate')
            .expect(200)).body.data.id

        await request.post('/api/validate')
            .send({ id, text: 'VALIDATE-text' })
            .expect(200, {
                error: false,
                data: true,
            })
    })

    it('能够在验证码错误时报错', async () => {
        const id = (await request.post('/api/generate')
            .expect(200)).body.data.id

        await request.post('/api/validate')
            .send({ id, text: 'incorrect' })
            .expect(400, {
                error: true,
                message: '验证码不正确',
            })
    })

    it('不能重复验证', async () => {
        const id = (await request.post('/api/generate')
            .expect(200)).body.data.id

        await request.post('/api/validate')
            .send({ id, text: 'validate-text' })

        await request.post('/api/validate')
            .send({ id, text: 'validate-text' })
            .expect(400, {
                error: true,
                message: '请刷新验证码',
            })
    })
})