
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('watch_logs', function (Blueprint $table) {
            $table->id();

            $table->unsignedBigInteger('elder_user_id');
            $table->unsignedBigInteger('videos_id');

            $table->dateTime('created_at')->useCurrent();
            $table->dateTime('updated_at')
                ->useCurrent()
                ->useCurrentOnUpdate();

            // 高齢者ユーザーとの外部キー制約
            $table->foreign('elder_user_id')
                ->references('id')
                ->on('elder_users');

            // 動画との外部キー制約
            $table->foreign('videos_id')
                ->references('id')
                ->on('videos');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('watch_logs');
    }
};
