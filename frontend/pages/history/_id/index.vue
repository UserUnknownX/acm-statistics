<template>
  <v-alert
    v-if="summaryError"
    type="error"
  >
    {{ summaryError }}
  </v-alert>
  <v-card-text
    v-else-if="summary === null"
    class="text-center"
  >
    <v-progress-circular
      :size="100"
      color="primary"
      indeterminate
      class="mt-10"
    />
  </v-card-text>
  <v-container
    v-else
    id="history-summary"
  >
    <v-list>
      <v-list-item v-if="summary.mainUsername">
        <v-list-item-content>
          <v-list-item-title><strong>Main username:</strong> {{ summary.mainUsername }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title><strong>SOLVED:</strong> {{ summary.solved }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title><strong>SUBMISSION:</strong> {{ summary.submission }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title><strong>Generated at</strong> {{ summary.generateTime }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-list>
    <v-simple-table>
      <template v-slot:default>
        <thead>
          <tr>
            <th
              class="text-left"
              scope="col"
            >
              Crawler
            </th>
            <th
              class="text-left"
              scope="col"
            >
              Username
            </th>
            <th
              class="text-left"
              scope="col"
            >
              Solved
            </th>
            <th
              class="text-left"
              scope="col"
            >
              Submission
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="item in workerSummaryList"
            :key="`${item.crawler}`"
          >
            <td scope="row">
              {{ item.crawler }}
            </td>
            <td>{{ item.username }}</td>
            <td>{{ item.solved }}</td>
            <td>{{ item.submissions }}</td>
          </tr>
        </tbody>
      </template>
    </v-simple-table>
    <bar-chart
      :chart-data="chartData"
      style="height: 300px"
    />
    <v-list dense>
      <v-subheader
        v-if="summary.summaryWarnings.length > 0"
        class="red--text"
      >
        WARNINGS
      </v-subheader>
      <v-list-item
        v-for="item in summary.summaryWarnings"
        :key="item"
      >
        <v-list-item-content>
          {{ crawlers[item.crawlerName].title }}:
          {{ item.content }}
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </v-container>
</template>

<script>
import _ from 'lodash'

import { getAbpErrorMessage } from '~/components/utils'
import BarChart from '~/components/BarChart'
import { PROJECT_TITLE } from '~/components/consts'

import HistoryToolbar from './-HistoryToolbar'
import GoHistoryPage from './-GoHistoryPage'

export default {
  components: {
    BarChart,
  },
  head: {
    title: `History - ${PROJECT_TITLE}`,
  },
  inject: ['changeLayoutConfig'],
  mounted() {
    this.changeLayoutConfig({
      title: 'Summary - Loading...',
    })
  },
  data() {
    return {
      summary: null,
      summaryError: null,
      crawlers: {},
    }
  },
  watch: {
    summary() {
      const dateFormatter = new Intl.DateTimeFormat()
      this.changeLayoutConfig({
        title: `Summary - ${dateFormatter.format(this.summary.generateTime)}`,
        topBarBeforeUserName: HistoryToolbar,
        topBarBeforeTitle: GoHistoryPage,
      })
    },
  },
  computed: {
    workerSummaryList() {
      // module not loaded
      if (!this.summary) {
        return []
      }

      const res = []
      for (const summary of this.summary.queryCrawlerSummaries) {
        const usernames = _.map(summary.usernames, item => {
          if (item.fromCrawlerName) {
            return `[${item.username} in ${this.crawlers[item.fromCrawlerName].title}]`
          } else {
            return item.username
          }
        })
        const isVirtualJudge = this.crawlers[summary.crawlerName].virtual_judge
        res.push({
          crawler: this.crawlers[summary.crawlerName].title + (isVirtualJudge ? ' (Not Merged)' : ''),
          username: usernames.join(', '),
          solved: summary.solved,
          submissions: summary.submission,
        })
      }
      return res
    },
    chartData() {
      const solvedList = _.map(this.workerSummaryList, 'solved')
      const submissionsList = _.map(this.workerSummaryList, 'submissions')
      return {
        labels: _.map(this.workerSummaryList, 'crawler'),
        datasets: [
          {
            label: 'solved',
            data: solvedList,
            backgroundColor: '#6699ff',
          },
          {
            label: 'submissions',
            data: submissionsList,
            backgroundColor: '#3d3d5c',
          },
        ],
      }
    },
  },
  async fetch() {

    try {

      const crawlersTask = this.$axios.$get('/api/crawlers')
      const summaryResult = await this.$axios.$get('/api/services/app/QueryHistory/GetQuerySummary', {
        params: {
          queryHistoryId: this.$route.params.id,
        },
      })

      this.crawlers = (await crawlersTask).data
      this.summary = summaryResult.result

      this.summary.generateTime = new Date(this.summary.generateTime)
    } catch (err) {
      this.summaryError = getAbpErrorMessage(err)
    }
  },
}
</script>
