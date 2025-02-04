#pragma once

#define LGFX_USE_V1

#include <nvs.h>
#include <nvs_flash.h>
#include <memory>
#include <esp_log.h>
#include <driver/i2c.h>
#include <soc/efuse_reg.h>
#include <soc/gpio_periph.h>
#include <soc/gpio_reg.h>
#include <soc/io_mux_reg.h>

#include "driver/uart.h"
#include "esp_timer.h"
#include "driver/gpio.h"
#include <string.h>
#include "esp_now.h"
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_wifi.h"
#include "esp_system.h"
#include "driver/adc.h"
#include "esp_adc_cal.h"
#include "esp_sleep.h"

#include <LovyanGFX.hpp>

#include <my_image_1.h>
#include <my_image_3.h>
//#include <connection_lost.h>

#define UART_NUM UART_NUM_1
#define UART_RX_BUFFER_SIZE 1024
#define TIMEOUT_THRESHOLD 3000  // 3 seconds, adjust as necessary
#define LCD_TIMEOUT_THRESHOLD 60000 // 20 seconds, adjust as necessary
#define STATUS_LED_GPIO 14  // Status LED is connected to GPIO 14
#define BUTTON_GPIO 13  // Button is connected to GPIO 0
#define EXTERNAL_POWER_GPIO 39  // External power is connected to GPIO 12
#define LED_GPIO 2      // LED is connected to GPIO 2
#define I2C_SDA_PIN 21 // I2C SDA pin
#define I2C_SCL_PIN 22 // I2C SCL pin
#define I2C_MASTER_NUM I2C_NUM_1    /*!< I2C port number for master dev */
#define I2C_MASTER_FREQ_HZ 100000   /*!< I2C master clock frequency */
#define SLEEP_THRESHOLD 600000 // 60 seconds, adjust as necessary

#define NVS_MAC_ADDRESS_KEY "mac_address"

#define COLOR_BLACK   0x0000
#define COLOR_WHITE   0xFFFF
#define COLOR_RED     0xF800
#define COLOR_GREEN   0x07E0
#define COLOR_BLUE    0x001F
#define COLOR_YELLOW  0xFFE0
#define COLOR_CYAN    0x07FF
#define COLOR_MAGENTA 0xF81F
#define COLOR_GRAY    0x8410
#define COLOR_DARKRED 0x8800
#define COLOR_DARKGREEN 0x0400
#define COLOR_DARKBLUE  0x0008

#define BATTERY_PIN       GPIO_NUM_35
#define ADC1_TEST_CHANNEL ADC1_CHANNEL_7
#define VREF              1100
#define FULL_BATTERY_VOLTAGE  4200 // in mV, adjust this based on your battery's specifications
#define EMPTY_BATTERY_VOLTAGE 3300 // in mV, adjust this based on your battery's specifications

// Constants for the polynomial fit
#define A_CONST 23.8
#define B_CONST -88.1
#define C_CONST 50

static esp_adc_cal_characteristics_t adc1_chars;

uint8_t current_status = 5;
static uint8_t led_state = 0;  // LED state
uint8_t previous_status = 0;
uint8_t peer_mac_address[6] = {0xb8, 0xd6, 0x1a, 0x42, 0x91, 0x2c};  // Peer MAC address
int64_t last_received_time = 0;
int64_t last_button_pressed_time = 0;
float external_voltage = 0.0;
bool external_power_connected = false;
float battery_charge_percentage = 0.0;

class LGFX : public lgfx::LGFX_Device
{
  lgfx::Panel_SSD1351     _panel_instance;
  lgfx::Bus_SPI       _bus_instance;   // SPIバスのインスタンス
  //lgfx::Light_PWM     _light_instance;
  //lgfx::Touch_FT5x06           _touch_instance; // FT5206, FT5306, FT5406, FT6206, FT6236, FT6336, FT6436

public:
  
  LGFX(void)
  {
    //PIN_PULLUP_DIS(IO_MUX_GPIO12_REG);

    { 
      auto cfg = _bus_instance.config();    // バス設定用の構造体を取得します。

      cfg.spi_host = VSPI_HOST;     // 使用するSPIを選択  ESP32-S2,C3 : SPI2_HOST or SPI3_HOST / ESP32 : VSPI_HOST or HSPI_HOST
      cfg.spi_mode = 3;             // SPI通信モードを設定 (0 ~ 3)
      cfg.freq_write = 10000000;    // 送信時のSPIクロック (最大80MHz, 80MHzを整数で割った値に丸められます)
      cfg.freq_read  = 5000000;    // 受信時のSPIクロック
      cfg.spi_3wire  = true;        // Set to true if reception is done on the MOSI pin
      cfg.use_lock   = true;        // Set true to use transaction lock
      cfg.dma_channel = SPI_DMA_CH_AUTO; // Set DMA channel to use (0=not use DMA / 1=1ch / 2=ch / SPI_DMA_CH_AUTO=auto setting)
      // With the ESP-IDF version upgrade, SPI_DMA_CH_AUTO (automatic setting) is recommended for the DMA channel. Specifying 1ch and 2ch is deprecated.
      cfg.pin_sclk = 18;            // SPIのSCLKピン番号を設定
      cfg.pin_mosi = 23;            // SPIのMOSIピン番号を設定
      cfg.pin_miso = -1;            // SPIのMISOピン番号を設定 (-1 = disable)
      cfg.pin_dc   = 19;            // SPIのD/Cピン番号を設定  (-1 = disable)
     // SDカードと共通のSPIバスを使う場合、MISOは省略せず必ず設定してください。


      _bus_instance.config(cfg);    // 設定値をバスに反映します。
      _panel_instance.setBus(&_bus_instance);      // バスをパネルにセットします。
    }

    { // 表示パネル制御の設定を行います。
      auto cfg = _panel_instance.config();    // 表示パネル設定用の構造体を取得します。

      cfg.pin_cs           =    5;  // CSが接続されているピン番号   (-1 = disable)
      //cfg.pin_cs           =    -1;  // CSが接続されているピン番号   (-1 = disable)
      cfg.pin_rst          =    25;  // RSTが接続されているピン番号  (-1 = disable)
      cfg.pin_busy         =    -1;  // BUSYが接続されているピン番号 (-1 = disable)

      // ※ 以下の設定値はパネル毎に一般的な初期値が設定されていますので、不明な項目はコメントアウトして試してみてください。

      cfg.panel_width      =   128;  // 実際に表示可能な幅
      cfg.panel_height     =   128;  // 実際に表示可能な高さ
      cfg.offset_x         =     0;  // パネルのX方向オフセット量
      cfg.offset_y         =     0;  // パネルのY方向オフセット量
      cfg.offset_rotation  =     0;  // 回転方向の値のオフセット 0~7 (4~7は上下反転)
      cfg.bus_shared       =  false;  // If the bus is shared with the SD card, set to true (bus control with drawJpgFile etc.)

      _panel_instance.config(cfg);
    }

/*

    { // バックライト制御の設定を行います。（必要なければ削除）
      auto cfg = _light_instance.config();    // バックライト設定用の構造体を取得します。

      cfg.pin_bl = 48;              // バックライトが接続されているピン番号
      cfg.invert = false;           // バックライトの輝度を反転させる場合 true
      cfg.freq   = 12000;           // バックライトのPWM周波数
      cfg.pwm_channel = 0;          // 使用するPWMのチャンネル番号

      _light_instance.config(cfg);
      _panel_instance.setLight(&_light_instance);  // バックライトをパネルにセットします。
    }



    { // タッチスクリーン制御の設定を行います。（必要なければ削除）
      auto cfg = _touch_instance.config();

      cfg.x_min      = 0;    // タッチスクリーンから得られる最小のX値(生の値)
      cfg.x_max      = 319;  // タッチスクリーンから得られる最大のX値(生の値)
      cfg.y_min      = 0;    // タッチスクリーンから得られる最小のY値(生の値)
      cfg.y_max      = 479;  // タッチスクリーンから得られる最大のY値(生の値)
      cfg.pin_int    = 40;   // INTが接続されているピン番号
      cfg.bus_shared = false; // 画面と共通のバスを使用している場合 trueを設定
      cfg.offset_rotation = 0;// 表示とタッチの向きのが一致しない場合の調整 0~7の値で設定

      cfg.i2c_port = 1;      // 使用するI2Cを選択 (0 or 1)
      cfg.i2c_addr = 0x38;   // I2Cデバイスアドレス番号
      cfg.pin_sda  = 38;     // SDAが接続されているピン番号
      cfg.pin_scl  = 39;     // SCLが接続されているピン番号
      cfg.freq = 400000;     // I2Cクロックを設定

      _touch_instance.config(cfg);
      _panel_instance.setTouch(&_touch_instance);  // タッチスクリーンをパネルにセットします。
    }
*/
    setPanel(&_panel_instance); // 使用するパネルをセットします。
  }
};

LGFX lcd;

void button_task(void* arg) {
    //gpio_set_direction(LED_GPIO, GPIO_MODE_OUTPUT);
    uint8_t status_saver = 0;
    
    while (1) {
        if (gpio_get_level((gpio_num_t)BUTTON_GPIO) == 1) { // Cnahged for NEW BUTTON
            printf("BUTTON PRESSED");
            char message[] = "BUTTON PRESSED";
            esp_now_send(peer_mac_address, (uint8_t*)message, sizeof(message));
            printf("Data sent to peer, MAC: %02X:%02X:%02X:%02X:%02X:%02X\n", peer_mac_address[0], peer_mac_address[1], peer_mac_address[2], peer_mac_address[3], peer_mac_address[4], peer_mac_address[5]);
            last_button_pressed_time = esp_timer_get_time() / 1000;
            lcd.invertDisplay(true);
            vTaskDelay(90 / portTICK_PERIOD_MS); // debounce delay
            lcd.invertDisplay(false);
            status_saver = current_status;
            current_status = 5;
            vTaskDelay(20 / portTICK_PERIOD_MS); // debounce delay
            current_status = status_saver;
            vTaskDelay(600 / portTICK_PERIOD_MS); // debounce delay
        }
        vTaskDelay(10 / portTICK_PERIOD_MS);  // polling delay
    }
}

void on_data_sent(const uint8_t *mac_addr, esp_now_send_status_t status) {
    //gpio_set_level(LED_GPIO, !gpio_get_level(LED_GPIO));
    printf("Data sent to peer\n");
}

void on_data_recv(const uint8_t *mac_addr, const uint8_t *data, int len) {
    // Make sure data is null-terminated
    char *str = (char *)malloc(len + 1);
    memcpy(str, data, len);
    str[len] = '\0';
    printf("MASTER:Received: %s\n", str);
    // Check if message contains status update
    char *status_loc = strstr(str, "Current status: ");
    char *mail_loc = strstr(str, "mail");
    char *contact_loc = strstr(str, "contact");
    char *application_loc = strstr(str, "application");
    char *emergency_loc = strstr(str, "emergency");
    char *link_loc = strstr(str, "link");
    char *call_loc = strstr(str, "call");
    if (status_loc != NULL) {
        // Extract the status value
        int status_value = atoi(status_loc + strlen("Current status: "));
        //printf("status: %d\n", status_value);
        if(status_value == 0 || status_value == 1){
            // Save the value to the corresponding variable
            current_status = status_value;
        }
    }
    // Check if message contains: mail, contact, application, emergency, link or call
    
    else if (mail_loc != NULL || contact_loc != NULL || application_loc != NULL || emergency_loc != NULL || link_loc != NULL || call_loc != NULL) {
        // Save the value to the corresponding variable
        //current_status = 2;
        //Convert received string to status code
        if (mail_loc != NULL){
            current_status = 2;
        }
        else if (contact_loc != NULL){
            current_status = 3;
        }
        else if (application_loc != NULL){
            current_status = 4;
        }
        else if (emergency_loc != NULL){
            current_status = 8;
        }
        else if (link_loc != NULL){
            current_status = 6;
        }
        else if (call_loc != NULL){
            current_status = 7;
        }
    }
    free(str);
    last_received_time = esp_timer_get_time() / 1000;

}

void init_wifi(){
    // Initialize WiFi
    tcpip_adapter_init();
    ESP_ERROR_CHECK(esp_event_loop_create_default());
    wifi_init_config_t cfg = WIFI_INIT_CONFIG_DEFAULT();
    ESP_ERROR_CHECK(esp_wifi_init(&cfg));
    ESP_ERROR_CHECK(esp_wifi_set_storage(WIFI_STORAGE_RAM));
    ESP_ERROR_CHECK(esp_wifi_set_mode(WIFI_MODE_STA));
    ESP_ERROR_CHECK(esp_wifi_start());

    // Get MAC address
    uint8_t mac[6];
    esp_efuse_mac_get_default(mac);

    // Print MAC address
    ESP_LOGI("WIFI", "MAC: %02X:%02X:%02X:%02X:%02X:%02X\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
}

void print_mac_address() {
    uint8_t mac[6];
    esp_efuse_mac_get_default(mac);
    ESP_LOGI("ADRESS", "MAC: %02X:%02X:%02X:%02X:%02X:%02X\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
}

void init_esp_now() {
    // Initialize ESP-NOW
    ESP_ERROR_CHECK(esp_now_init());
    ESP_ERROR_CHECK(esp_now_register_send_cb(on_data_sent));
    ESP_ERROR_CHECK(esp_now_register_recv_cb(on_data_recv));

    // Add peer
    esp_now_peer_info_t peer_info;
    memset(&peer_info, 0, sizeof(peer_info));
    peer_info.channel = 0;  
    peer_info.encrypt = false;
    memcpy(peer_info.peer_addr, peer_mac_address, 6);
    ESP_ERROR_CHECK(esp_now_add_peer(&peer_info));
}

esp_err_t init_nvs() {
    esp_err_t ret = nvs_flash_init();
    if (ret == ESP_ERR_NVS_NO_FREE_PAGES || ret == ESP_ERR_NVS_NEW_VERSION_FOUND) {
      ESP_ERROR_CHECK(nvs_flash_erase());
      ret = nvs_flash_init();
    }
    ESP_LOGI("NVS", "NVS init: %s", esp_err_to_name(ret));
    return ret;
}

esp_err_t save_mac_address(const uint8_t* mac_address) {
    nvs_handle_t nvs_handle;
    esp_err_t err = nvs_open("storage", NVS_READWRITE, &nvs_handle);
    if (err != ESP_OK) {
        
        ESP_LOGE("NVS", "Error opening NVS: %s", esp_err_to_name(err));
        return err;
    }

    err = nvs_set_blob(nvs_handle, NVS_MAC_ADDRESS_KEY, mac_address, 6);
    if (err != ESP_OK) {
        ESP_LOGE("NVS", "Error saving MAC address to NVS: %s", esp_err_to_name(err));
    }

    nvs_close(nvs_handle);
    return err;
}

esp_err_t read_mac_address(uint8_t* mac_address) {
    nvs_handle_t nvs_handle;
    esp_err_t err = nvs_open("storage", NVS_READWRITE, &nvs_handle);
    if (err != ESP_OK) {
        ESP_LOGE("NVS", "Error opening NVS: %s", esp_err_to_name(err));
        return err;
    }

    size_t required_size = 6;
    err = nvs_get_blob(nvs_handle, NVS_MAC_ADDRESS_KEY, mac_address, &required_size);
    if (err == ESP_OK && required_size == 6) {
        ESP_LOGI("NVS", "Read MAC address from NVS");
    } else if (err == ESP_ERR_NVS_NOT_FOUND) {
        ESP_LOGI("NVS", "MAC address not found in NVS.");
        err = nvs_set_blob(nvs_handle, NVS_MAC_ADDRESS_KEY, mac_address, 6);
        if (err != ESP_OK) {
            ESP_LOGE("NVS", "Error saving MAC address to NVS: %s", esp_err_to_name(err));
        }
    } else {
        ESP_LOGE("NVS", "Error reading MAC address from NVS: %s", esp_err_to_name(err));
    }

    nvs_close(nvs_handle);
    return err;
}

// Function to check if message contains one of prefixes stored in array
bool check_prefix(char *message, char **prefixes, int prefixes_size) {
    for (int i = 0; i < prefixes_size; i++) {
        if (strncmp(message, prefixes[i], strlen(prefixes[i])) == 0) {
            return true;
        }
    }
    return false;
}

void uart_task(void *pvParameters) {
    uint8_t uart_rx_buffer[UART_RX_BUFFER_SIZE];
    uint8_t mac_address[6];
    uint8_t mac_address_received = 0;

    // Set up UART configuration
    uart_config_t uart_config = {
        .baud_rate = 115200,
        .data_bits = UART_DATA_8_BITS,
        .parity = UART_PARITY_DISABLE,
        .stop_bits = UART_STOP_BITS_1,
        .flow_ctrl = UART_HW_FLOWCTRL_DISABLE
    };
    uart_param_config(UART_NUM_0, &uart_config);
    uart_driver_install(UART_NUM_0, UART_RX_BUFFER_SIZE * 2, 0, 0, NULL, 0);

    char *mac_prefix = "MAC: ";
    char *reboot_prefix = "REBOOT";
    char *ping_prefix = "PING";
    char *mail_prefix = "mail";
    char *contact_prefix = "contact";
    char *application_prefix = "application";
    char *emergency_prefix = "emergency";
    char *link_prefix = "link";
    char *call_prefix = "call";
        // Create an array of prefixes
    char *prefixes[] = {mail_prefix, contact_prefix, application_prefix, emergency_prefix, link_prefix, call_prefix};

    while (1) {
        int len = uart_read_bytes(UART_NUM_0, uart_rx_buffer, UART_RX_BUFFER_SIZE, 100 / portTICK_PERIOD_MS);
        if (len > 0) {
            // Process the received UART data here
            // Assume the data received is the MAC address in the format "MAC: xx:xx:xx:xx:xx:xx"
            
            if (strncmp((char *)uart_rx_buffer, mac_prefix, strlen(mac_prefix)) == 0) {
                if (sscanf((char *)uart_rx_buffer + strlen(mac_prefix), "%02hhX:%02hhX:%02hhX:%02hhX:%02hhX:%02hhX",
                           &mac_address[0], &mac_address[1], &mac_address[2], &mac_address[3], &mac_address[4], &mac_address[5]) == 6) {
                    // MAC address successfully extracted from UART message
                    mac_address_received = 1;
                    ESP_LOGI("UART", "Received MAC address:");
                    for (int i = 0; i < 6; i++) {
                        ESP_LOGI("UART", "%02X ", mac_address[i]);
                    }
                    ESP_LOGI("UART", "\n");

                    // Save the MAC address using the function from the previous answer
                    esp_err_t nvs_err = save_mac_address(mac_address);
                    if (nvs_err == ESP_OK) {
                        ESP_LOGI("UART", "MAC address saved to NVS.");
                    }
                }
            }
            else if (strncmp((char *)uart_rx_buffer, reboot_prefix, strlen(reboot_prefix)) == 0) {
                ESP_LOGI("UART", "Rebooting...");
                esp_restart();
            }
            else if (strncmp((char *)uart_rx_buffer, ping_prefix, strlen(ping_prefix)) == 0) {
                ESP_LOGI("UART", "Ping received!");
                last_received_time = esp_timer_get_time() / 1000;
            }
            else if (check_prefix((char *)uart_rx_buffer, prefixes, sizeof(prefixes) / sizeof(prefixes[0]))) {
                last_received_time = esp_timer_get_time() / 1000;
                // Set crresponding status code
                if (strncmp((char *)uart_rx_buffer, mail_prefix, strlen(mail_prefix)) == 0) {
                    current_status = 2;
                }
                else if (strncmp((char *)uart_rx_buffer, application_prefix, strlen(application_prefix)) == 0) {
                    current_status = 4;
                }
                else if (strncmp((char *)uart_rx_buffer, link_prefix, strlen(link_prefix)) == 0) {
                    current_status = 6;
                }
                else if (strncmp((char *)uart_rx_buffer, call_prefix, strlen(call_prefix)) == 0) {
                    current_status = 7;
                }
                else if (strncmp((char *)uart_rx_buffer, link_prefix, strlen(link_prefix)) == 0) {
                    current_status = 5;
                }
                
            }

            // Clear the UART buffer
            memset(uart_rx_buffer, 0, sizeof(uart_rx_buffer));
        }
    }
}


void init_display(void)
{
  lcd.init();

  lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);

  lcd.setRotation(4);

  lcd.fillScreen(TFT_BLACK);
}

void check_timeout_task(void *pvParameter) {
    char data_str[20] = {0};
    sprintf(data_str, "CONNECTION FAIL!");
    ESP_LOGI("TIMEOUT", "Timeout task started!");
    uint8_t timeout_message_counter = 0;
    int64_t current_time = esp_timer_get_time() / 1000;
    int64_t time_diff = current_time - last_received_time;
    while (1) {
        current_time = esp_timer_get_time() / 1000;
        time_diff = current_time - last_received_time;
        if (time_diff > TIMEOUT_THRESHOLD) {
                current_status = 3; // Timeout status code
                timeout_message_counter++;
                if (timeout_message_counter == 100){
                    ESP_LOGI("TIMEOUT", "No responce from display device!");
                    timeout_message_counter = 0;
                }  
        }
        //ESP_LOGI("TIMEOUT", "Current status: %" PRId64, time_diff);
        vTaskDelay(100 / portTICK_PERIOD_MS);  // check every second
    }
}

void send_periodic_ping(void *pvParameter) {
    char data_str[20] = {0};
    sprintf(data_str, "PING");
    ESP_LOGI("PING", "PING task started!");
    while (1) {
        esp_now_send(peer_mac_address, (uint8_t*)data_str, sizeof(data_str));
        ESP_LOGI("PING", "PING sent to peer, MAC: %02X:%02X:%02X:%02X:%02X:%02X", peer_mac_address[0], peer_mac_address[1], peer_mac_address[2], peer_mac_address[3], peer_mac_address[4], peer_mac_address[5]);
        vTaskDelay(1000 / portTICK_PERIOD_MS);  // send every 1000 ms
    }
}

void draw_power_saving_screen(void){
    lcd.clearDisplay();
    lcd.setTextColor(COLOR_WHITE);
    lcd.drawString("POWER SAVING MODE", 14, 24);
    lcd.drawString("PUSH TO WAKE UP", 19, 54);
    if (battery_charge_percentage > 50){
        lcd.setTextColor(COLOR_GREEN);
    }
    else if (battery_charge_percentage > 20){
        lcd.setTextColor(COLOR_YELLOW);
    }
    else {
        lcd.setTextColor(COLOR_RED);
    }
    lcd.drawString("BATTERY: ", 27, 104);
    lcd.drawNumber(battery_charge_percentage, 77, 104);
    lcd.drawString("%", 100, 104);
    if (external_power_connected == true){
        lcd.setTextColor(COLOR_YELLOW);
        lcd.drawString("CHARGING", 40, 114);
    }
}

void check_lcd_timeout_task(void *pvParameter) {
    ESP_LOGI("LCD_TIMEOUT", "LCD timeout task started!");
    int64_t current_time = esp_timer_get_time() / 1000;
    uint8_t saver_mode_flag = 0;
    uint8_t saver_mode_flag_previous = 0;
    float previous_battery_charge_percentage = 0;
    bool previous_external_power_connected = false;
    while (1) {
        current_time = esp_timer_get_time() / 1000;
        if (current_time - last_button_pressed_time > LCD_TIMEOUT_THRESHOLD) {
                saver_mode_flag = 1;
        }
        else saver_mode_flag = 0;
        
            if(saver_mode_flag == 0){
            //lcd.setBrightness(128);
            }
            else if(saver_mode_flag == 1 && saver_mode_flag_previous == 0){
                //draw_power_saving_screen();
                if (battery_charge_percentage != previous_battery_charge_percentage || external_power_connected != previous_external_power_connected){
                //draw_power_saving_screen();
                ESP_LOGI("LCD_TIMEOUT", "LCD timeout!");
                }
                //lcd.setBrightness(0);
            }
            else if(saver_mode_flag == 1 && saver_mode_flag_previous == 1){
                if (battery_charge_percentage != previous_battery_charge_percentage || external_power_connected != previous_external_power_connected){
                //draw_power_saving_screen();
                ESP_LOGI("LCD_TIMEOUT", "LCD timeout!");
                }
                //lcd.setBrightness(0);
            }
            previous_battery_charge_percentage = battery_charge_percentage;
            previous_external_power_connected = external_power_connected;
        
        saver_mode_flag_previous = saver_mode_flag;

        vTaskDelay(100 / portTICK_PERIOD_MS);  // check every second
    }
}


void drawBatteryIcon(int x, int y, int width, int height, float batteryPercentage, bool whiteBorder = true) {
    if (whiteBorder) {
        // 1. Draw the main body of the battery
        lcd.drawRect(x, y, width, height, TFT_WHITE); // replace TFT_WHITE with the desired color

        // 2. Draw the positive terminal
        int terminalWidth = width * 0.1; // 10% of battery width for the terminal
        lcd.drawRect(x + width, y + (height * 0.25), terminalWidth, height * 0.5, TFT_WHITE); // Adjust the 0.25 and 0.5 factors if needed
    }
    else {
        // 1. Draw the main body of the battery
        lcd.drawRect(x, y, width, height, TFT_BLACK); // replace TFT_WHITE with the desired color

        // 2. Draw the positive terminal
        int terminalWidth = width * 0.1; // 10% of battery width for the terminal
        lcd.drawRect(x + width, y + (height * 0.25), terminalWidth, height * 0.5, TFT_BLACK); // Adjust the 0.25 and 0.5 factors if needed
    }
    

    // 3. Fill battery based on percentage
    int fillWidth = (width - 2) * batteryPercentage; // -2 for the border
    if (battery_charge_percentage > 50){
        lcd.fillRect(x + 1, y + 1, fillWidth, height - 2, TFT_GREEN); // Adjust the color (TFT_GREEN) if desired
    }
    else if (battery_charge_percentage > 20){
        lcd.fillRect(x + 1, y + 1, fillWidth, height - 2, TFT_YELLOW); // Adjust the color (TFT_YELLOW) if desired
    }
    else {
        lcd.fillRect(x + 1, y + 1, fillWidth, height - 2, TFT_RED); // Adjust the color (TFT_RED) if desired
    }
    

    // 4. Draw charging state symbol (a simple lightning bolt) if charging
    if (external_power_connected == true) {
        int barWidth = width / 4; // The width of the charging bar
        int startX = x + (width / 2) - (barWidth / 2);
        lcd.fillRect(startX, y + 2, barWidth, height - 4, TFT_YELLOW);
    }
}

void displayConnectionLost() {
    // Clear the screen
    lcd.fillScreen(TFT_BLACK);

    // Draw the red triangle (equilateral triangle for simplicity)
    int triangleSize = 70; // You can adjust this as needed
    int centerX = lcd.width() / 2;
    int centerY = lcd.height() / 2.5;

    int halfTriangle = triangleSize / 2;
    int height = (sqrt(3) * triangleSize) / 2;

    lcd.fillTriangle(
        centerX, centerY - height / 2, 
        centerX - halfTriangle, centerY + height / 2, 
        centerX + halfTriangle, centerY + height / 2,
        TFT_RED
    );

    // Draw the exclamation mark in the center of the triangle
    lcd.setTextColor(TFT_BLACK); // Black color for the exclamation mark to contrast the red triangle
    lcd.setTextSize(5);  // Adjust as needed
    textdatum_t current_allignment = lcd.getTextDatum();  // Center alignment
    lcd.setTextDatum(TC_DATUM);  // Center alignment
    lcd.drawString("!", centerX + 2, centerY - 9);  // Adjust the Y offset as needed

    // Display "Connection Lost" text below the triangle
    lcd.setTextColor(TFT_WHITE);
    lcd.setTextSize(1.3); 
    lcd.setTextDatum(TC_DATUM); // Center alignment
    lcd.drawString("Connection Lost", centerX, centerY + height / 2 + 10);  // Adjust the Y offset as needed
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);
    lcd.setTextDatum(current_allignment); 
}

void drawRingingPhone() {
    uint16_t screenWidth = lcd.width();
    uint16_t screenHeight = lcd.height();

    uint16_t phoneWidth = screenWidth / 2;
    uint16_t phoneHeight = screenHeight / 1.5;
    uint16_t phoneX = screenWidth / 2 - phoneWidth / 2;
    uint16_t phoneY = screenHeight / 5.5;

    // Sound waves (semicircles or arcs)
    uint16_t waveColor = lcd.color888(0, 0, 255); // Blue
    uint16_t waveX = phoneX + phoneWidth / 1.1;
    uint16_t waveY = phoneY;
    for (int i = 0; i < 3; i++) {
        int r = phoneWidth / 3 + i * phoneWidth / 10;
        lcd.drawCircle(waveX, waveY, r, waveColor);
    }

    // Phone body (rectangle)
    uint16_t phoneColor = lcd.color888(30, 30, 30); // Dark grey
    lcd.fillRect(phoneX, phoneY, phoneWidth, phoneHeight, phoneColor);

    // Phone screen (smaller rectangle)
    uint16_t screenColor = lcd.color888(0, 0, 230); // Light Gray
    uint16_t screenX = phoneX + phoneWidth / 8;
    uint16_t screenY = phoneY + phoneHeight / 8;
    uint16_t phoneScreenWidth = phoneWidth * 3 / 4;
    uint16_t phoneScreenHeight = phoneHeight / 2;
    lcd.fillRect(screenX, screenY, phoneScreenWidth, phoneScreenHeight, screenColor);

    // Phone buttons (small rectangles within the phone body)
    uint16_t buttonColor = lcd.color888(0, 0, 0); // Black
    uint16_t buttonY = screenY + phoneScreenHeight + phoneHeight / 16;
    for (int i = 0; i < 3; i++) {
        lcd.fillRect(screenX, buttonY + i*(phoneScreenHeight / 5.5), phoneScreenWidth, phoneScreenHeight / 16, buttonColor);
    }

    // Write text under Icon
    lcd.setTextColor(COLOR_WHITE);
    lcd.setTextSize(1.5);
    textdatum_t current_allignment = lcd.getTextDatum();  // Center alignment
    lcd.setTextDatum(TC_DATUM);  // Center alignment
    lcd.drawString("CALL", phoneX + phoneWidth/2, phoneY + phoneHeight + 5);
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);
    lcd.setTextDatum(current_allignment);

}


void drawBrowserIcon() {
    uint16_t screenWidth = lcd.width();
    uint16_t screenHeight = lcd.height();

    uint16_t globeRadius = screenWidth / 3;
    uint16_t x = screenWidth / 2; // centering the globe horizontally
    uint16_t y = screenHeight / 2; // centering the globe vertically
    
    // Drawing a Globe (Circle) as the base of the browser icon
    uint16_t globeColor = lcd.color888(0, 0, 255); // Blue for water
    lcd.fillCircle(x, y, globeRadius, globeColor);

    // Drawing some continents or landmasses (smaller random circles) on the globe
    uint16_t landColor = lcd.color888(0, 255, 0); // Green for land
    lcd.fillCircle(x - globeRadius / 4, y - globeRadius / 4, globeRadius / 4, landColor);
    lcd.fillCircle(x + globeRadius / 3, y + globeRadius / 3, globeRadius / 5, landColor);
    lcd.fillCircle(x - globeRadius / 3, y + globeRadius / 4, globeRadius / 6, landColor);
    lcd.fillCircle(x + globeRadius / 4, y - globeRadius / 5, globeRadius / 7, landColor);

    // Drawing Equator and Prime Meridian Lines
    uint16_t lineColor = lcd.color888(255, 255, 255); // White
    lcd.drawFastHLine(x - globeRadius, y, 2 * globeRadius, lineColor); // Equator
    lcd.drawFastVLine(x, y - globeRadius, 2 * globeRadius, lineColor); // Prime Meridian

    // Drawing a Cursor (Triangle) on top of the Globe
    //uint16_t cursorColor = lcd.color888(255, 0, 0); // Red
    //lcd.fillTriangle(x, y - globeRadius, x + globeRadius / 2, y, x - globeRadius / 2, y, cursorColor);
    
    // Drawing a circle around the globe to represent the browser
    uint16_t borderColor = lcd.color888(0, 0, 0); // Black
    lcd.drawCircle(x, y, globeRadius + 5, borderColor);

    //Write text under Icon
    lcd.setTextColor(COLOR_WHITE);
    lcd.setTextSize(1.5);
    textdatum_t current_allignment = lcd.getTextDatum();  // Center alignment
    lcd.setTextDatum(TC_DATUM);  // Center alignment
    lcd.drawString("BROWSER", x, y + globeRadius + 5);
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);
    lcd.setTextDatum(current_allignment);
    
}



void drawEmailIcon() {
    uint16_t screenWidth = lcd.width();
    uint16_t screenHeight = lcd.height();

    uint16_t iconWidth = screenWidth / 1.4; // width of the icon
    uint16_t iconHeight = screenHeight / 1.6; // height of the icon
    
    uint16_t x = (screenWidth - iconWidth) / 2; // centering the icon horizontally
    uint16_t y = (screenHeight - iconHeight) / 2; // centering the icon vertically
    
    uint16_t envelopeColor = lcd.color888(255, 255, 255); // White color for envelope
    uint16_t lineColor = lcd.color888(0, 0, 0); // Black color for lines
    
    // Drawing envelope rectangle
    lcd.fillRect(x, y, iconWidth, iconHeight, envelopeColor);
    
    // Drawing lines for envelope details
    uint16_t cx = x + iconWidth / 2;  // center x of the icon
    uint16_t cy = y + iconHeight / 2; // center y of the icon
    lcd.drawLine(x, y, cx, cy, lineColor); // top left to center
    lcd.drawLine(x + iconWidth, y, cx, cy, lineColor); // top right to center
    lcd.drawLine(x, y + iconHeight, cx, cy, lineColor); // bottom left to center
    lcd.drawLine(x + iconWidth, y + iconHeight, cx, cy, lineColor); // bottom right to center

    //Write text under Icon
    lcd.setTextColor(COLOR_WHITE);
    lcd.setTextSize(1.5);
    textdatum_t current_allignment = lcd.getTextDatum();  // Center alignment
    lcd.setTextDatum(TC_DATUM);  // Center alignment
    lcd.drawString("MAIL", x + iconWidth / 2, y + iconHeight + 5);
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);
    lcd.setTextDatum(current_allignment);

}


void drawAppIcon() {
    uint16_t screenWidth = lcd.width();
    uint16_t screenHeight = lcd.height();
    
    uint16_t iconSize = std::min(screenWidth, screenHeight) / 1.7; // making icon size adaptive
    uint16_t x = (screenWidth - iconSize) / 2; // centering the icon horizontally
    uint16_t y = (screenHeight - iconSize) / 2; // centering the icon vertically
    uint16_t radius = iconSize / 6; // corner radius
    
    // Define color for icon elements
    uint16_t backgroundColor = lcd.color888(255, 165, 0); // Orange
    uint16_t circleColor = lcd.color888(255, 255, 255); // White
    
    // Drawing rounded rectangle filled with a gradient color
    lcd.fillRoundRect(x, y, iconSize, iconSize, radius, backgroundColor);
    
    // Drawing a white circle in the center
    uint16_t cr = iconSize / 4; // radius of the circle
    lcd.fillCircle(x + iconSize / 2, y + iconSize / 2, cr, circleColor);

    //Write text under Icon
    lcd.setTextColor(COLOR_WHITE);
    lcd.setTextSize(1.5);
    textdatum_t current_allignment = lcd.getTextDatum();  // Center alignment
    lcd.setTextDatum(TC_DATUM);  // Center alignment
    lcd.drawString("APP", x + iconSize / 2, y + iconSize + 5);
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);
    lcd.setTextDatum(current_allignment);
}



void status_display(void* arg) {
    printf("Status display task started\n");
    while (1) {
        if (current_status != previous_status)
        {
            switch (current_status)
            {
            case 0:
                // Out of the office
                //lcd.fillScreen(COLOR_RED);
                //lcd.powerSaveOff();
                //lcd.setBrightness(128);
                lcd.clearDisplay();
                lcd.drawBmp(pic_out_of_office, pic_out_of_office_len, 0, 0);
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, false);
                break;
            case 1:
                // In the office
                //lcd.fillScreen(COLOR_GREEN);
                //lcd.powerSaveOff();
                lcd.clearDisplay();
                lcd.drawBmp(pic_in_office, pic_in_office_len, 0, 0);
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, false);
                break;
            case 2:
                // Email icon
                lcd.clearDisplay();
                drawEmailIcon();
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, true);
                //lcd.powerSaveOn();    
                break;
            case 3:
                // Timeout
                lcd.clearDisplay();
                displayConnectionLost();
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, true);                   
                break;
            case 4:
                // Application
                lcd.clearDisplay();
                drawAppIcon();
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, true);
                break;
            case 5:
                // Browse
                //lcd.clearDisplay();
                //drawBrowserIcon();
                //drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, false);
                break;
            case 6:
                // Link
                lcd.clearDisplay();
                drawBrowserIcon();
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, true);
                break;
            case 7:
                // Call
                lcd.clearDisplay();
                drawRingingPhone();
                drawBatteryIcon(5, 5, 20, 10, battery_charge_percentage / 100.0, true);
                break;
            case 8:
               //Emergency
               break;

            default:
                lcd.clearDisplay();
                // Display error message
                lcd.setTextColor(COLOR_WHITE);
                lcd.drawString("ERROR", 0, 0);
                break;
            }
        }
        previous_status = current_status;
        vTaskDelay(10 / portTICK_PERIOD_MS);  // delay for 1 second
    }
    vTaskDelay(10 / portTICK_PERIOD_MS);
}


float getBatteryPercentage(float voltage) {
    float SoC = A_CONST * voltage * voltage + B_CONST * voltage + C_CONST;
    ESP_LOGI("BATTERY MONITOR", "SoC: %.2f%%", SoC);
    
    if (SoC > 100) SoC = 100;
    if (SoC < 0) SoC = 0;

    return SoC;
}

void battery_monitor_task(void* arg) {
    adc1_config_width(ADC_WIDTH_BIT_12);
    adc1_config_channel_atten(ADC1_CHANNEL_7, ADC_ATTEN_DB_11);  // GPIO 35
    char message[30] = {0};
    float battery_percentage = 0;
    float voltage = 0;
    while(1) {
        //int raw = adc1_get_raw(ADC1_CHANNEL_7);
        //float voltage = ((float)raw / 4095.0) * 2.0 * VREF / 1000.0;  // Convert raw reading to voltage
        voltage = (esp_adc_cal_raw_to_voltage(adc1_get_raw(ADC1_CHANNEL_7), &adc1_chars) * 2) / 1000.0;
        battery_percentage = getBatteryPercentage(voltage); // Multiply by two because of the voltage divider in circuit
        ESP_LOGI("BATTERY MONITOR", "Battery Voltage: %.2f V, Estimated SoC: %.2f%%", voltage, battery_percentage);
        battery_charge_percentage = battery_percentage;

        vTaskDelay(60000 / portTICK_PERIOD_MS);  // Delay for a minute
        
        //sprintf(message, "Button battery charge: %.2f V", battery_charge_percentage);
        //esp_now_send(peer_mac_address, (uint8_t*)message, sizeof(message));
        //printf("Data sent to peer, MAC: %02X:%02X:%02X:%02X:%02X:%02X\n", peer_mac_address[0], peer_mac_address[1], peer_mac_address[2], peer_mac_address[3], peer_mac_address[4], peer_mac_address[5]);

    }
}

void external_power_monitor_task(void* arg) {
    
    // Configure ADC
    adc1_config_width(ADC_WIDTH_BIT_12);
    adc1_config_channel_atten(ADC1_CHANNEL_3, ADC_ATTEN_DB_11);  // GPIO 39
    uint8_t message_timer = 0;
    while(1) {
        //int raw = adc1_get_raw(ADC1_CHANNEL_3);
        //float voltage = ((float)raw / 4095.0) * 2.0 * VREF / 1000.0;  // Convert raw reading to voltage
        uint32_t voltage = esp_adc_cal_raw_to_voltage(adc1_get_raw(ADC1_CHANNEL_3), &adc1_chars);
        external_voltage = (voltage / 1000.0) / 0.67;

        if (external_voltage > 4.0) {
            external_power_connected = true;
        } else {
            external_power_connected = false;
        }

        vTaskDelay(100 / portTICK_PERIOD_MS);

        message_timer++;
        if (message_timer == 600) {
            //ESP_LOGI("EXTERNAL POWER MONITOR", "External Power Voltage: %.2f V", external_voltage);
            //char message[20] = {0};
            //sprintf(message, "External power: %.2f V", external_voltage);
            //esp_now_send(peer_mac_address, (uint8_t*)message, sizeof(message));
            //printf("Data sent to peer, MAC: %02X:%02X:%02X:%02X:%02X:%02X\n", peer_mac_address[0], peer_mac_address[1], peer_mac_address[2], peer_mac_address[3], peer_mac_address[4], peer_mac_address[5]);
            message_timer = 0;
        }
    }
}

static void IRAM_ATTR button_isr_handler(void* arg) {
    // Optionally, we can handle debouncing or any other logic here if required.
}

void config_button_gpio(void){
    // Configure the GPIO as input with pull-up resistor
    gpio_config_t io_conf;
    io_conf.intr_type = GPIO_INTR_NEGEDGE; // Interrupt on falling edge (button press)
    io_conf.pin_bit_mask = 1ULL << BUTTON_GPIO;
    io_conf.mode = GPIO_MODE_INPUT;
    io_conf.pull_up_en = GPIO_PULLUP_DISABLE;
    io_conf.pull_down_en = GPIO_PULLDOWN_ENABLE;
    gpio_config(&io_conf);
    // Install the ISR handler for the button GPIO
    gpio_install_isr_service(0);
    gpio_isr_handler_add((gpio_num_t)BUTTON_GPIO, button_isr_handler, NULL);
}

void espnow_deinit() {
    // Unregister send and receive callback functions
    esp_now_unregister_recv_cb();
    esp_now_unregister_send_cb();

    // Stop ESP-NOW
    esp_now_deinit();

    // Stop Wi-Fi
    esp_wifi_stop();
    
    // Deinit Wi-Fi to free up the resources
    esp_wifi_deinit();

    // Deinit the event loop
    esp_event_loop_delete_default();
}



void drawSleepModeIcon(int x, int y, int radius) {
    lcd.clearDisplay();
    lcd.fillCircle(x, y, radius, TFT_GOLD); // Draw a full circle representing the moon background
    lcd.fillCircle(x + (radius / 4), y, radius - (radius / 4), TFT_BLACK); // Draw a smaller circle on the right side to make it look like a crescent moon

    // Draw the "sleeping" text below the crescent moon
    lcd.setTextColor(TFT_WHITE);
    lcd.setTextSize(2);
    int textWidth = lcd.textWidth("Sleeping");
    lcd.drawString("Sleeping", x - textWidth/2, y + radius + 5); // The '5' ensures there's a small gap between the icon and the text
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);
}


void drawSunrise() {
    // Clear the screen
    //lcd.fillScreen(TFT_BLACK);
    lcd.clearDisplay();

    // Dimensions
    int16_t screenWidth = lcd.width();
    int16_t screenHeight = lcd.height();
    int16_t horizonY = screenHeight * 2 / 4;  // Set horizon 2/4 down the screen.

    // Draw the horizon
    lcd.fillRect(0, 0, screenWidth, screenHeight - horizonY, 0x00008B); 

    // Draw the sun partially below the horizon
    int16_t sunRadius = screenWidth / 3; // Adjust as needed.
    lcd.fillCircle(screenWidth / 2, horizonY, sunRadius, TFT_RED);

    // Draw water
    lcd.fillRect(0, 64, screenWidth, screenHeight - horizonY, TFT_BLUE); 
    // Sky gradient (basic example without much blending)
    /*
    for (int16_t y = 128; y > horizonY; y--) {
        uint16_t color = lcd.color888(8 + y * 247 / horizonY, 8 + y * 140 / horizonY, 255);
        lcd.drawLine(0, y, screenWidth, y, color);
    }
    */
   // Draw the "sleeping" text below the crescent moon
    lcd.setTextColor(TFT_RED);
    lcd.setTextSize(2);
    int textWidth = lcd.textWidth("Waking up");
    lcd.drawString("Waking up", 12, 64 + 19); // The '5' ensures there's a small gap between the icon and the text
    lcd.setTextSize((std::max(lcd.width(), lcd.height()) + 255) >> 8);  
}




void sleep_monitor_task(void* arg) {
    int64_t current_time = esp_timer_get_time() / 1000;
    ESP_LOGI("SLEEP_TASK", "Sleep monitor task started!");
    esp_sleep_enable_ext0_wakeup((gpio_num_t)BUTTON_GPIO, 1);
    while(1) {
        current_time = esp_timer_get_time() / 1000;
        if (current_time - last_button_pressed_time > SLEEP_THRESHOLD) {
            if (!external_power_connected){
                // No button press for 1 minutes, enter light sleep
                ESP_LOGI("SLEEP_TASK", "Entering light sleep");
                
                //esp_now_deinit();
                
                vTaskDelay(20 / portTICK_PERIOD_MS);
                drawSleepModeIcon(64, 50, 30);
                esp_deep_sleep_start();
                ESP_LOGI("SLEEP_TASK", "Woke up from light sleep, restarting...");
                //current_status = 4;
                vTaskDelay(10 / portTICK_PERIOD_MS);
                //drawSunrise();
                esp_restart();
                
                //last_button_pressed_time = esp_timer_get_time() / 1000;
                
                //lcd.clearDisplay();
            }      
        }
        vTaskDelay(1000 / portTICK_PERIOD_MS);  // check every second
    }
}


extern "C" void app_main() {
    // Initialize display
    init_display();
    drawSunrise();
    // Configure button GPIO
    config_button_gpio();
    // Callibrate adc
    esp_adc_cal_characterize(ADC_UNIT_1, ADC_ATTEN_DB_11, ADC_WIDTH_12Bit, 0, &adc1_chars);
    // Initialize NVS
    ESP_ERROR_CHECK(init_nvs());
    // Read MAC address from NVS
    ESP_ERROR_CHECK(read_mac_address(peer_mac_address));
    // Initialize wifi
    init_wifi();
    // Initialize ESP-NOW and register callbacks
    init_esp_now();
    // Initialize timer
    last_received_time = esp_timer_get_time() / 1000;
    // Create tasks
    xTaskCreate(button_task, "button_task", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(uart_task, "uart_task", 4096 * 2, NULL, 1, NULL);
    xTaskCreate(check_timeout_task, "check_timeout_task", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(status_display, "status_display", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(check_lcd_timeout_task, "check_lcd_timeout_task", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(battery_monitor_task, "battery_monitor_task", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(external_power_monitor_task, "external_power_monitor_task", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(sleep_monitor_task, "sleep_monitor_task", 1024 * 2, NULL, 1, NULL);
    xTaskCreate(send_periodic_ping, "send_periodic_ping", 1024 * 2, NULL, 1, NULL);
    print_mac_address();
}
