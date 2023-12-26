#include <stdio.h>

int main() {
    int n;
    scanf_s("%d", &n);
    unsigned long long num1, num2;

    for (int i = 0; i < n; i++) {
        unsigned long long k;
        int m;
        int maxdiff, mindiff;
        scanf_s("%llu %d", &k, &m);

        unsigned long long divisor = 2147483648;
        int count1 = 0;
        unsigned long long num = k;
        for (int j = 0; j < 32; j++) {
            int temp = num / divisor;
            if (temp == 1) {
                count1++;
            }
            num %= divisor;
            divisor /= 2;
        }

        for (int j = 0; j < m; j++) {
            unsigned long long can;
            int diff = 0;
            scanf_s("%llu", &can);
            if (j == 0) {
                printf("Case %d:\n", i + 1);
            }
            unsigned long long divisor1 = 2147483648;
            unsigned long long temp1 = can;
            int count2 = 0;
            for (int l = 0; l < 32; l++) {
                int temp = temp1 / divisor1;
                if (temp == 1) {
                    count2++;
                }
                printf("%d", temp);
                temp1 %= divisor1;
                divisor1 /= 2;
            }
            printf("\n");

            if (count1 > count2) {
                diff = count1 - count2;
            }
            else if (count2 > count1) {
                diff = count2 - count1;
            }


            if (j == 0) {
                num1 = can;
                num2 = can;
                maxdiff = diff;
                mindiff = diff;
            }
            else if (j >= 1) {
                if (mindiff > diff || (mindiff == diff && can < num1)) {
                    num1 = can;
                    mindiff = diff;
                }
                if (maxdiff < diff || (maxdiff == diff && can > num2)) {
                    num2 = can;
                    maxdiff = diff;
                }
            }

        }

        unsigned long long divisor3 = 2147483648;
        unsigned long long num3 = k;
        for (int j = 0; j < 32; j++) {
            int temp = num3 / divisor3;
            printf("%d", temp);
            num3 %= divisor3;
            divisor3 /= 2;
        }
        printf("\n");

        printf("Number with Minimum Difference: %llu\n", num1);
        printf("Number with Maximum Difference: %llu\n", num2);


    }

    return 0;
}